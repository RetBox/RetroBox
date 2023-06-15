using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Fabric.Prebuilt;
using RetroBox.Manager.CoreData;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;
using IOPath = System.IO.Path;

namespace RetroBox.Manager.Views
{
    public partial class NewVmWindow : FixWindow
    {
        private string? _initDir;

        public NewVmWindow()
        {
            InitializeComponent();
        }

        private NewVmViewModel Model => (DataContext as NewVmViewModel)!;

        private async void BrowseDir_OnClick(object? sender, RoutedEventArgs e)
        {
            var f = _initDir;
            var folder = await Dialogs.AskToOpenFolder(this, "Select folder as root!", f);
            if (folder == null)
                return;
            _initDir = folder;
            OnNameChange(NameBox, Model.Name);
        }

        private void Dialog_OnOpened(object? sender, EventArgs e)
        {
            CreateBtn.IsEnabled = false;
            _initDir = Model.Folder;
            TemplBox.SelectedIndex = 0;
            NameBox.OnTextChange(OnNameChange);
        }

        private void OnNameChange(TextBox nameBox, string newText)
        {
            newText = newText.Trim();
            Model.Folder = IOPath.Combine(_initDir!, newText);
            CreateBtn.IsEnabled = !string.IsNullOrWhiteSpace(newText);
        }

        private void Create_OnClick(object? sender, RoutedEventArgs e)
        {
            var vmName = Model.Name;
            var vmFolder = Model.Folder;
            var vmTempl = (IMetaTemplate)TemplBox.SelectedItem!;

            // TODO Create?!

            Close(ButtonResult.Ok);
            ReloadVmResults();
        }

        private void ReloadVmResults()
        {
            var parent = this.GetParent<MainWindow>();
            if (parent?.DataContext is not MainWindowViewModel model)
                return;
            model.SearchMachines(CancellationToken.None);
        }

        private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Cancel);
        }
    }
}