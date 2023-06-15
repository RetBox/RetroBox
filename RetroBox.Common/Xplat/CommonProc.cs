using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;
using RetroBox.API.Xplat;

namespace RetroBox.Common.Xplat
{
    public abstract class CommonProc : IPlatProc
    {
        protected static List<string> ToArgs(StartBoxArg a)
        {
            var args = new List<string>();
            if (!string.IsNullOrWhiteSpace(a.Config))
            {
                args.Add("--config");
                args.Add(a.Config);
            }
            if (a.FullScreen)
            {
                args.Add("--fullscreen");
            }
            if (!string.IsNullOrWhiteSpace(a.LogFile))
            {
                args.Add("--logfile");
                args.Add(a.LogFile);
            }
            if (!string.IsNullOrWhiteSpace(a.VmPath))
            {
                args.Add("--vmpath");
                args.Add(a.VmPath);
            }
            if (!string.IsNullOrWhiteSpace(a.RomPath))
            {
                args.Add("--rompath");
                args.Add(a.RomPath);
            }
            if (a.Settings)
            {
                args.Add("--settings");
            }
            if (!string.IsNullOrWhiteSpace(a.VmName))
            {
                args.Add("--vmname");
                args.Add(a.VmName);
            }
            return args;
        }

        public static async Task Fuck(StartBoxArg a)
        {
            var cmd = Cli.Wrap(a.ExeFile!).WithValidation(CommandResultValidation.None);
            if (a.WorkDir != null) cmd = cmd.WithWorkingDirectory(a.WorkDir);
            if (a.Vars != null) cmd = cmd.WithEnvironmentVariables(a.Vars!);
            if (ToArgs(a) is { Count: >= 1 } args) cmd = cmd.WithArguments(args, escape: true);

            var observed = cmd.Observe();
            await observed.ForEachAsync(cmdEvent =>
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent started:
                        break;
                    case StandardOutputCommandEvent stdOut:
                        break;
                    case StandardErrorCommandEvent stdErr:
                        break;
                    case ExitedCommandEvent exited:
                        break;
                    default:
                        throw new InvalidOperationException(nameof(cmdEvent));
                }
            });
        }
    }
}