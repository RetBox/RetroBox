using System;
using System.Threading;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Manager.CoreData;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;

namespace RetroBox.Manager.Views
{
    public partial class NewVmWindow : FixWindow
    {
        public NewVmWindow()
        {
            InitializeComponent();
        }

        private void BrowseDir_OnClick(object? sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void Dialog_OnOpened(object? sender, EventArgs e)
        {
        }

        private void Create_OnClick(object? sender, RoutedEventArgs e)
        {
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