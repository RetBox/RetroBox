using System;
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
        }

        private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Cancel);
        }

        private void Apply_OnClick(object? sender, RoutedEventArgs e)
        {
            Close(ButtonResult.Ok);
        }
    }
}