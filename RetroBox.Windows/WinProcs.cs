using System;
using System.Collections.Generic;
using CliWrap;
using RetroBox.Common.Commands;
using RetroBox.Common.Messages;
using RetroBox.Common.Xplat;
using RetroBox.Windows.Core;

namespace RetroBox.Windows
{
    internal sealed class WinProcs : CommonProc
    {
        private readonly WinLoop _loop;

        public WinProcs()
        {
            _loop = new WinLoop(Receive);
        }

        public override Command Build(StartBoxArg a)
        {
            if (a.Settings)
            {
                return base.Build(a);
            }

            var id = WinHandles.GetTempId(a.VmPath!);
            var idString = $"{id:X}".PadLeft(16, '0');

            var handle = _loop.Handle;
            var hWndHex = $"{handle.ToInt64():X}".PadLeft(16, '0');

            WinHandles.Create(a.CallId, id, handle, _loop);

            a.ExtraArgs = new List<string> { "--hwnd", $"{idString},{hWndHex}" };
            var cmd = base.Build(a);
            return cmd;
        }

        public override void CleanUp(string tag)
        {
            WinHandles.Destroy(tag);
        }

        private void Receive(object sender, IVmMessage message)
        {
            // TODO
        }

        public override void Send(string tag, IVmCommand cmd)
        {
            var cmdByte = ConvertToMsg(cmd);
            WinHandles.Write(tag, cmdByte);
        }

        private static (int, IntPtr, IntPtr) ConvertToMsg(IVmCommand cmd)
        {
            switch (cmd)
            {
                case SettingsVCmd:
                    return (0x8889, IntPtr.Zero, IntPtr.Zero);
                case PauseVCmd:
                    return (0x8890, IntPtr.Zero, IntPtr.Zero);
                case SoftResetVCmd:
                    return (0x8894, IntPtr.Zero, IntPtr.Zero);
                case HardResetVCmd:
                    return (0x8892, IntPtr.Zero, IntPtr.Zero);
                case RequestOffVCmd:
                    return (0x8893, IntPtr.Zero, IntPtr.Zero);
                case ForceOffVCmd:
                    return (0x8893, new IntPtr(1), IntPtr.Zero);
                default:
                    throw new InvalidOperationException($"{cmd} ?!");
            }
        }
    }
}