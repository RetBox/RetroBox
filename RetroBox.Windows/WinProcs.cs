using System;
using System.Collections.Generic;
using System.Threading;
using CliWrap;
using RetroBox.Common.Commands;
using RetroBox.Common.Messages;
using RetroBox.Common.Special;
using RetroBox.Common.Xplat;
using RetroBox.Windows.Core;
using static RetroBox.Common.Tools.Apps;
using static RetroBox.Windows.Core.WinImports;

// ReSharper disable NotAccessedField.Local

namespace RetroBox.Windows
{
    internal sealed class WinProcs : CommonProc
    {
        private readonly WinLoop _loop;

        public WinProcs()
        {
            _loop = new WinLoop();
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

            WinHandles.Create(a.CallId, id, handle);

            a.ExtraArgs = new List<string> { "--hwnd", $"{idString},{hWndHex}" };
            var cmd = base.Build(a);
            return cmd;
        }

        public override void CleanUp(string tag)
        {
            WinHandles.Destroy(tag);
        }

        public override void Send(string tag, IVmCommand cmd)
        {
            var cmdByte = ConvertToMsg(cmd);
            WinHandles.Write(tag, cmdByte);
        }

        public override void Send(IntPtr hWnd, IMgrCommand cmd)
        {
            var cmdText = ConvertToMsg(cmd);
            WinHandles.Write(hWnd, cmdText);
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

        private static string ConvertToMsg(IMgrCommand cmd)
        {
            switch (cmd)
            {
                case LinkStartMCmd lc:
                    return lc.VmName;
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

        private static Mutex? _mutex;

        public override bool IsFirstInstance()
        {
            _mutex = new Mutex(true, MainTitle, out var firstInstance);
            return firstInstance;
        }

        public override IntPtr RestoreExistingWindow()
        {
            var hWnd = FindWindow(null, MainTitle);
            ShowWindow(hWnd, ShowWindowEnum.Show);
            ShowWindow(hWnd, ShowWindowEnum.Restore);
            SetForegroundWindow(hWnd);

            hWnd = FindWindow(null, _loop.Title);
            return hWnd;
        }

        public override string Setup()
        {
            return $"{_loop.Handle.ToInt64():X2}";
        }
    }
}