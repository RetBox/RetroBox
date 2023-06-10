using System;
using RetroBox.Manager.ViewCore;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Manager.ViewModels;

namespace RetroBox.Manager.Views
{
    public partial class AboutWindow : FixWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void About_OnOpened(object? sender, EventArgs e)
        {
            var model = (AboutViewModel)DataContext!;
            var dll = model.Dll;
            Title = $"About {dll.Product}";
        }

        private void About_OnClose(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Ok);
        }
    }
}