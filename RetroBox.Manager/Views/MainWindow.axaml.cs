using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Fabric.Boxes;
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
                if (Model.CurrentMachine is { } machine)
                {
                    var machineDir = machine.Folder;
                    const string previewName = "preview.png";
                    var previewFile = IOPath.Combine(machineDir, previewName);
                    File.Copy(fileName, previewFile, overwrite: true);
                    machine.Preview = previewName;
                    TriggerMachines();
                }
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
            Model.CurrentMachine = (IMetaMachine?)added;
        }

        private MainWindowViewModel Model => (MainWindowViewModel)DataContext!;

        private async void ChangeName_OnClick(object? sender, RoutedEventArgs e)
        {
            if (!await ChangeParticle(nameof(Model.CurrentMachine.Name),
                    Model.CurrentMachine?.Name,
                    t => Model.CurrentMachine!.Name = t))
                return;
            TriggerMachines();
        }

        private async void ChangeDesc_OnClick(object? sender, RoutedEventArgs e)
        {
            if (!await ChangeParticle(nameof(Model.CurrentMachine.Description),
                    Model.CurrentMachine?.Description,
                    t => Model.CurrentMachine!.Description = t))
                return;
            TriggerMachines();
        }

        private void TriggerMachines()
        {
            var old = Model.CurrentMachine;
            if (old == null)
                return;
            var index = Model.AllMachines.IndexOf(old);
            Model.AllMachines.RemoveAt(index);
            Model.AllMachines.Insert(index, old);
            Model.CurrentMachine = null;
            Model.CurrentMachine = old;
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
    }
}