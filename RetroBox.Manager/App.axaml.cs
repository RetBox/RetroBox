using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RetroBox.Manager.ViewModels;
using RetroBox.Manager.Views;
using System.Threading.Tasks;
using RetroBox.Manager.ViewCore;

namespace RetroBox.Manager
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SplashScreenViewModel splashModel;
                var splash = new SplashScreen { DataContext = splashModel = new SplashScreenViewModel() };
                desktop.MainWindow = splash;
                splash.Show();

                try
                {
                    splashModel.StartupMessage = "Searching for devices...";
                    await Task.Delay(1000, splashModel.CancellationToken);
                    splashModel.StartupMessage = "Connecting to device #1...";
                    await Task.Delay(2000, splashModel.CancellationToken);
                    splashModel.StartupMessage = "Configuring device...";
                    await Task.Delay(2000, splashModel.CancellationToken);
                }
                catch (TaskCanceledException)
                {
                    splash.Close();
                    return;
                }

                var main = new MainWindow { DataContext = new MainWindowViewModel() };
                desktop.MainWindow = main;
                main.Show();

                splash.Close();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}