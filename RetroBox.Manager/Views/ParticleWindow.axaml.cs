using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Manager.ViewCore;
using RetroBox.Manager.ViewModels;

namespace RetroBox.Manager.Views
{
    public partial class ParticleWindow : FixWindow
    {
        public ParticleWindow()
        {
            InitializeComponent();
        }

        private void Edit_OnOpened(object? sender, EventArgs e)
        {
            var model = (ParticleViewModel)DataContext!;
            Title = model.Title;
            ValBox.Focus();
        }

        private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Cancel);
        }

        private void Apply_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Ok);
        }

        private void Text_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key is Key.Return or Key.Enter)
                Apply_OnClick(sender, new RoutedEventArgs(e.RoutedEvent));
            if (e.Key is Key.Escape)
                Cancel_OnClick(sender, new RoutedEventArgs(e.RoutedEvent));
        }
    }
}