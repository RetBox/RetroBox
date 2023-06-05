using Avalonia;
using Avalonia.ReactiveUI;
using System;
using JetBrains.Annotations;

namespace RetroBox.Manager
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = BuildAvaloniaApp();
            app.StartWithClassicDesktopLifetime(args);
        }

        [UsedImplicitly]
        public static AppBuilder BuildAvaloniaApp()
        {
            var bld = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
            return bld;
        }
    }
}