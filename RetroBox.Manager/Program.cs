using Avalonia;
using Avalonia.ReactiveUI;
using System;
using JetBrains.Annotations;
using RetroBox.Manager.Tools;
using RetroBox.Common.Special;
using RetroBox.Fabric;
using RetroBox.Manager.CoreLogic;

namespace RetroBox.Manager
{
    internal static class Program
    {
        public static Options? Opt;

        [STAThread]
        public static void Main(string[] args)
        {
            var opt = Opt = CmdUtil.Parse(args);
            var proc = Platforms.My.GetProcs();
            if (!proc.IsFirstInstance())
            {
                var handle = proc.RestoreExistingWindow();
                if (!string.IsNullOrWhiteSpace(opt.VmName))
                {
                    proc.Send(handle, new LinkStartMCmd(opt.VmName));
                }
                return;
            }
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