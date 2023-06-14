using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RetroBox.Manager.ViewModels;
using RetroBox.Manager.Views;
using System.Threading.Tasks;
using RetroBox.Manager.CoreData;

namespace RetroBox.Manager
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SplashScreenViewModel splashModel;
                var splash = new SplashScreen { DataContext = splashModel = new SplashScreenViewModel() };
                desktop.MainWindow = splash;
                splash.Show();

                var mainView = new MainWindowViewModel();
                try
                {
                    splashModel.StartupMessage = "Finding software...";
                    mainView.SearchSoftware(splashModel.CancellationToken);

                    splashModel.StartupMessage = "Finding machines...";
                    mainView.SearchMachines(splashModel.CancellationToken);
                }
                catch (TaskCanceledException)
                {
                    splash.Close();
                    return;
                }

                var main = new MainWindow { DataContext = mainView };
                desktop.MainWindow = main;
                main.Show();

                splash.Close();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}