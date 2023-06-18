using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CliWrap.EventStream;
using MessageBox.Avalonia.Enums;
using RetroBox.API;
using RetroBox.API.Data;
using RetroBox.Common.Commands;
using RetroBox.Common.Messages;
using RetroBox.Common.Special;
using RetroBox.Common.Tools;
using RetroBox.Common.Xplat;
using RetroBox.Fabric;
using RetroBox.Fabric.Boxes;
using RetroBox.Fabric.Config;
using RetroBox.Manager.Boxes;
using RetroBox.Manager.CoreLogic;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;
using IOPath = System.IO.Path;
using MIcon = MessageBox.Avalonia.Enums.Icon;

namespace RetroBox.Manager.Views
{
    public partial class MainWindow : FixWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Main_OnOpened(object? sender, EventArgs e)
        {
            Title = Apps.MainTitle;
            RegisterListen();
            RegisterDragDrop();
        }

        private void RegisterListen()
        {
            var plat = Platforms.My;
            var proc = plat.GetProcs();
            proc.Receive(OnManagerMessage);
            proc.Receive(OnMachineMessage);

            var setup = proc.Setup();
            Trace.WriteLine(setup);
        }

        private void RegisterDragDrop()
        {
            PreviewArea.AddHandler(DragDrop.DragOverEvent, OnPreviewDragOver);
            PreviewArea.AddHandler(DragDrop.DropEvent, OnPreviewDragDrop);
        }

        private void OnPreviewDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects &= DragDropEffects.Copy | DragDropEffects.Link;
            if (!PreviewArea.Equals(sender) || !e.Data.Contains(DataFormats.FileNames))
                e.DragEffects = DragDropEffects.None;
        }

        private void OnPreviewDragDrop(object? sender, DragEventArgs e)
        {
            if (!PreviewArea.Equals(sender) || !e.Data.Contains(DataFormats.FileNames))
                return;
            var fileNames = e.Data.GetFileNames()?.ToArray();
            if (fileNames?.Length == 1)
            {
                var fileName = fileNames[0];
                OnPreviewDragDrop(fileName);
            }
        }

        private void OnPreviewDragDrop(string fileName)
        {
            if (Model.CurrentMachine is not { } machine) 
                return;
            var machineDir = machine.GetFolder()!;
            const string previewName = "preview.png";
            var previewFile = IOPath.Combine(machineDir, previewName);
            File.Copy(fileName, previewFile, overwrite: true);
            machine.Preview = previewName;
            TriggerMachines("Preview");
        }

        private async void PreviewArea_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Right)
            {
                var fileName = await Dialogs.AskToOpenFile(this, "Choose the screenshot to load!",
                    "png", "Image files");
                if (fileName != null)
                    OnPreviewDragDrop(fileName);
            }
        }

        private async void MenuUpdate_OnClick(object? sender, RoutedEventArgs e)
        {
            var emu = new EmuUpdateWindow { DataContext = new EmuUpdateViewModel() };
            await emu.ShowDialogFor(this);
        }

        private void MenuExit_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Ok);
        }

        private async void MenuAbout_OnClick(object? sender, RoutedEventArgs e)
        {
            var about = new AboutWindow { DataContext = new AboutViewModel() };
            await about.ShowDialogFor(this);
        }

        private void Machines_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var added = e.AddedItems.OfType<object>().FirstOrDefault();
            if (added == null)
                return;
            Model.CurrentMachine = (MetaMachine?)added;
        }

        private MainWindowViewModel Model => (MainWindowViewModel)DataContext!;

        private async void ChangeName_OnClick(object? sender, RoutedEventArgs e)
        {
            if (!await ChangeParticle(nameof(Model.CurrentMachine.Name),
                    Model.CurrentMachine?.Name,
                    t => Model.CurrentMachine!.NameTxt = t))
                return;
            TriggerMachines("Name");
        }

        private async void ChangeDesc_OnClick(object? sender, RoutedEventArgs e)
        {
            if (!await ChangeParticle(nameof(Model.CurrentMachine.Description),
                    Model.CurrentMachine?.Description,
                    t => Model.CurrentMachine!.DescriptionTxt = t))
                return;
            TriggerMachines("Comment");
        }

        private void TriggerMachines(string txt)
        {
            Model.Status = $"{txt} changed.";
        }

        private async Task<bool> ChangeParticle(string title, string? value, Action<string> setter)
        {
            if (value == null)
                return false;
            var initial = value == Machines.None ? string.Empty : value;
            var model = new ParticleViewModel
            {
                Title = $@"Edit ""{value}""", Key = title, Val = initial
            };
            var par = new ParticleWindow { DataContext = model };
            if (await par.ShowDialogFor(this) != ButtonResult.Ok)
                return false;
            var currentVal = model.Val;
            if (initial.Equals(currentVal))
                return false;
            setter(currentVal);
            return true;
        }

        private void AddExist_OnClick(object? sender, RoutedEventArgs e)
        {
            // TODO
        }

        private async void CreateNew_OnClick(object? sender, RoutedEventArgs e)
        {
            var cfg = Configs.Default;
            var machRoot = cfg.MachineRoot;

            var model = new NewVmViewModel
            {
                Templates = Model.AllTemplates, Name = string.Empty, Folder = machRoot
            };
            var dialog = new NewVmWindow { DataContext = model };
            await dialog.ShowDialogFor(this);
        }

        private (MetaMachine mach, FoundExe exe, FoundRom rom)? GetEmuAndRom()
        {
            if (Model.CurrentMachine is { } m
                && EmuCombo.SelectedItem is FoundExe e
                && RomCombo.SelectedItem is FoundRom r)
                return (m, e, r);
            return null;
        }

        private async Task<MetaMachine?> GetMachineByTag(string tag)
        {
            var machines = await Events.Invoke<IEnumerable<MetaMachine>>(() => Model.AllMachines);
            var mach = machines.FirstOrDefault(m => m.Tag == tag);
            return mach;
        }

        private void ConfigureThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            if (mach.Status == MachineState.Stopped)
            {
                var cfg = Configs.Default;
                var id = Monitor.GenerateId();
                var arg = new StartBoxArg
                {
                    TempDir = cfg.TempPath,
                    CallId = id,
                    ExeFile = current.exe.File,
                    RomPath = current.rom.Path,
                    VmPath = mach.GetFolder(),
                    VmName = mach.Name,
                    Settings = true
                };
                var plat = Platforms.My;
                var proc = plat.GetProcs();
                var core = proc.Build(arg);
                mach.Tag = id;
                core.RunThis(id, ConfigureThis_OnEvent);
                return;
            }
            if (mach.Status == MachineState.Running)
            {
                var plat = Platforms.My;
                var proc = plat.GetProcs();

                var vCmd = new SettingsVCmd();
                proc.Send(mach.Tag!, vCmd);
            }
        }

        private async void ConfigureThis_OnEvent(object sender, string tag, CommandEvent d)
        {
            var mach = await GetMachineByTag(tag);
            if (mach?.Tag == null)
                return;
            if (d is StartedCommandEvent or InitEvent)
            {
                mach.Status = MachineState.Waiting;
                return;
            }
            if (d is ExitedCommandEvent or CompleteEvent)
            {
                mach.Status = MachineState.Stopped;

                ReloadFromConfig(mach);
            }
        }

        private static void ReloadFromConfig(MetaMachine mach)
        {
            var former = mach.M;
            if (former == null)
                return;
            mach.M = Machines.ReadConfig(former.Name, former.File);
        }

        private void StartThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            if (mach.Status == MachineState.Stopped)
            {
                var cfg = Configs.Default;
                var id = Monitor.GenerateId();
                var arg = new StartBoxArg
                {
                    TempDir = cfg.TempPath,
                    CallId = id,
                    ExeFile = current.exe.File,
                    RomPath = current.rom.Path,
                    VmPath = mach.GetFolder(),
                    VmName = mach.Name
                };
                var plat = Platforms.My;
                var proc = plat.GetProcs();
                var core = proc.Build(arg);
                mach.Tag = id;
                core.RunThis(id, StartThis_OnEvent);
            }
        }

        private async void StartThis_OnEvent(object sender, string tag, CommandEvent d)
        {
            var mach = await GetMachineByTag(tag);
            if (mach?.Tag == null)
                return;
            if (d is StartedCommandEvent or InitEvent)
            {
                mach.Status = MachineState.Running;
                return;
            }
            if (d is ExitedCommandEvent or CompleteEvent)
            {
                mach.Status = MachineState.Stopped;

                var plat = Platforms.My;
                var proc = plat.GetProcs();
                proc.CleanUp(mach.Tag);
            }
        }

        private void PauseThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            if (mach.Status == MachineState.Running)
            {
                var plat = Platforms.My;
                var proc = plat.GetProcs();

                var vCmd = new PauseVCmd();
                proc.Send(mach.Tag!, vCmd);

                mach.Status = MachineState.Paused;
            }
        }

        private async void StopThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            if (mach.Status != MachineState.Running)
                return;
            var res = await Dialogs.ShowMessageBox("Invoke normal shutdown?", "Request or force off",
                MIcon.Question, ButtonEnum.YesNoCancel);
            var isYes = res == ButtonResult.Yes;
            var isNo = res == ButtonResult.No;
            if (!isYes && !isNo)
                return;
            var plat = Platforms.My;
            var proc = plat.GetProcs();
            IVmCommand vCmd = isYes ? new RequestOffVCmd() : new ForceOffVCmd();
            proc.Send(mach.Tag!, vCmd);
        }

        private void KillThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            var myTag = mach.Tag;
            var proc = Monitor.FindProcess(myTag);
            if (proc == null)
                return;
            using (proc)
            {
                proc.CloseMainWindow();
                proc.Kill();
            }
        }

        private void ResumeThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            if (mach.Status == MachineState.Paused)
            {
                var plat = Platforms.My;
                var proc = plat.GetProcs();

                var vCmd = new PauseVCmd();
                proc.Send(mach.Tag!, vCmd);

                mach.Status = MachineState.Running;
            }
        }

        private async void ResetThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;
            var mach = current.mach;
            if (mach.Status != MachineState.Running)
                return;
            var res = await Dialogs.ShowMessageBox("Press Ctrl-Alt-Del?", "Soft or hard reset",
                MIcon.Question, ButtonEnum.YesNoCancel);
            var isYes = res == ButtonResult.Yes;
            var isNo = res == ButtonResult.No;
            if (!isYes && !isNo)
                return;
            var plat = Platforms.My;
            var proc = plat.GetProcs();
            IVmCommand vCmd = isYes ? new SoftResetVCmd() : new HardResetVCmd();
            proc.Send(mach.Tag!, vCmd);
        }

        private void OnManagerMessage(object? sender, IMgrMessage message)
        {
            throw new InvalidOperationException();
        }

        private async void OnMachineMessage(object? rawSender, IVmMessage message)
        {
            var sender = ((IHasTag)rawSender!).Tag!;
            var mach = await GetMachineByTag(sender);
            if (mach?.Tag == null)
                return;
            if (message is EmuInitVMsg)
            {
                mach.Status = MachineState.Running;
                return;
            }
            if (message is EmuShutdownVMsg)
            {
                mach.Status = MachineState.Stopped;
                return;
            }
            if (message is VmPausedVMsg)
            {
                mach.Status = MachineState.Paused;
                return;
            }
            if (message is VmResumedVMsg)
            {
                mach.Status = MachineState.Running;
                return;
            }
            if (message is DialogOpenedVMsg)
            {
                mach.Status = MachineState.Waiting;
                return;
            }
            if (message is DialogClosedVMsg)
            {
                mach.Status = MachineState.Running;
                return;
            }
            throw new InvalidOperationException(message.GetType().FullName);
        }
    }
}