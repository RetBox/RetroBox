using System.Threading;
using Avalonia.Interactivity;
using MessageBox.Avalonia.Enums;
using RetroBox.Manager.ViewCore;

namespace RetroBox.Manager.Views
{
    public partial class ProgressWindow : FixWindow
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
        {
            _cts.Cancel();
            Close(ButtonResult.Cancel);
        }

        private readonly CancellationTokenSource _cts = new();

        public CancellationToken CancellationToken => _cts.Token;
    }
}