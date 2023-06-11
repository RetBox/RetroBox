using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Fabric.Boxes;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;

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
            var added = e.AddedItems[0];
            var model = (MainWindowViewModel)DataContext!;
            model.CurrentMachine = (IMetaMachine?)added;
        }
    }
}