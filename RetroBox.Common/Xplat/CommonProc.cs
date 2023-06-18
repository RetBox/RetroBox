using System.Collections.Generic;
using CliWrap;
using RetroBox.API.Xplat;
using RetroBox.Common.Commands;

namespace RetroBox.Common.Xplat
{
    public abstract class CommonProc : IPlatProc
    {
        protected static List<string> ToArgs(StartBoxArg a, IEnumerable<string>? extra)
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
            if (extra != null)
            {
                args.AddRange(extra);
            }
            return args;
        }

        public virtual Command Build(StartBoxArg a)
        {
            var cmd = Cli.Wrap(a.ExeFile!).WithValidation(CommandResultValidation.None);
            if (a.WorkDir != null)
                cmd = cmd.WithWorkingDirectory(a.WorkDir);
            if (a.Vars != null)
                cmd = cmd.WithEnvironmentVariables(a.Vars!);
            if (ToArgs(a, a.ExtraArgs) is { Count: >= 1 } args)
                cmd = cmd.WithArguments(args, escape: true);
            return cmd;
        }

        public abstract void CleanUp(string tag);

        public abstract void Send(string tag, IVmCommand cmd);
    }
}