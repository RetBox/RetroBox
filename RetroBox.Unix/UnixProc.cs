using System;
using System.Collections.Generic;
using System.IO;
using CliWrap;
using RetroBox.Common.Commands;
using RetroBox.Common.Xplat;

namespace RetroBox.Unix
{
    public abstract class UnixProc : CommonProc
    {
        public override Command Build(StartBoxArg a)
        {
            if (a.Settings)
            {
                return base.Build(a);
            }

            var name = a.VmName?.GetHashCode();
            var socketName = $"{name}_{Environment.ProcessId}";

            var socketFile = Path.Combine(a.TempDir!, socketName);
            UnixSockets.Create(a.CallId, socketFile);

            a.Vars = new Dictionary<string, string?> { { "86BOX_MANAGER_SOCKET", socketName } };
            var cmd = base.Build(a);
            return cmd;
        }

        public override void CleanUp(string tag)
        {
            UnixSockets.Destroy(tag);
        }

        public override void Send(string tag, IVmCommand cmd)
        {
            var cmdTxt = ConvertToTxt(cmd);
            UnixSockets.Write(tag, cmdTxt);
        }

        private static string ConvertToTxt(IVmCommand cmd)
        {
            switch (cmd)
            {
                case SettingsVCmd:
                    return "showsettings";
                case PauseVCmd:
                    return "pause";
                case SoftResetVCmd:
                    return "cad";
                case HardResetVCmd:
                    return "reset";
                case RequestOffVCmd:
                    return "shutdown";
                case ForceOffVCmd:
                    return "shutdownnoprompt";
                default:
                    throw new InvalidOperationException($"{cmd} ?!");
            }
        }
    }
}