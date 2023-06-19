using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CliWrap;
using RetroBox.Common.Commands;
using RetroBox.Common.Messages;
using RetroBox.Common.Special;
using RetroBox.Common.Xplat;

// ReSharper disable NotAccessedField.Local

namespace RetroBox.Unix
{
    public abstract class UnixProc : CommonProc
    {
        protected readonly UnixLoop _loop = new();

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

        public override void Send(IntPtr tag, IMgrCommand cmd)
        {
            switch (cmd)
            {
                case LinkStartMCmd lc:
                    _loop.WriteStartVm(tag, lc.VmName);
                    break;
                default:
                    throw new InvalidOperationException($"{cmd} ?!");
            }
        }

        public override void Receive(EventHandler<IVmMessage> action)
        {
            _loop.CallbackV = action;
        }

        public override void Receive(EventHandler<IMgrMessage> action)
        {
            _loop.CallbackM = action;
        }

        private IntPtr _lastEnemy;

        public override bool IsFirstInstance()
        {
            var entry = Assembly.GetEntryAssembly()?.Location;
            if (entry != null)
            {
                var exeName = Path.GetFileNameWithoutExtension(entry);
                var processes = Process.GetProcessesByName(exeName);
                var myProcId = Environment.ProcessId;
                foreach (var proc in processes)
                    if (proc.Id != myProcId)
                    {
                        _lastEnemy = new IntPtr(proc.Id);
                        return false;
                    }
            }
            return true;
        }

        public override IntPtr RestoreExistingWindow()
        {
            return _lastEnemy;
        }

        public override string Setup()
        {
            return $"{Environment.ProcessId:X2}";
        }
    }
}