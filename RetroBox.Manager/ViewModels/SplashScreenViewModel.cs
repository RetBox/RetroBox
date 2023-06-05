using ReactiveUI;
using System.Threading;

namespace RetroBox.Manager.ViewModels
{
    public class SplashScreenViewModel : ViewModelBase
    {
        private string _startupMessage = "Starting application...";

        public string StartupMessage
        {
            get => _startupMessage;
            set => this.RaiseAndSetIfChanged(ref _startupMessage, value);
        }

        public void Cancel()
        {
            StartupMessage = "Cancelling...";
            _cts.Cancel();
        }

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public CancellationToken CancellationToken => _cts.Token;
    }
}