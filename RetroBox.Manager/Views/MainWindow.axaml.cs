using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.API.Data;
using RetroBox.Common.Xplat;
using RetroBox.Fabric.Boxes;
using RetroBox.Fabric.Config;
using RetroBox.Manager.Boxes;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;
using IOPath = System.IO.Path;

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
            RegisterDragDrop();
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
            throw new NotImplementedException(); // TODO
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

        private async void ConfigureThis_OnClick(object? sender, RoutedEventArgs e)
        {
            if (GetEmuAndRom() is not { } current)
                return;

            var arg = new StartBoxArg
            {
                ExeFile = current.exe.File,
                RomPath = current.rom.Path,
                VmPath = current.mach.GetFolder(),
                VmName = current.mach.Name,
                Settings = true
            };
            await CommonProc.Fuck(arg);

            // TODO
        }
    }
}