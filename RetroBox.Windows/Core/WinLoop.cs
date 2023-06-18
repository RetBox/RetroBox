using System.ComponentModel;
using System.Runtime.InteropServices;
using RetroBox.Common.Messages;
using static RetroBox.Windows.Core.WinImports;
using I = System.IntPtr;

// ReSharper disable NotAccessedField.Local

namespace RetroBox.Windows.Core
{
    internal class WinLoop
    {
        private readonly WinMsgHandler? _callback;
        private readonly WndProc? _wndDelegate;

        public WinLoop(WinMsgHandler? callback, string title = "RetroBox Manager Secret")
        {
            _callback = callback;
            if (_callback == null)
                return;
            Handle = CreateMessageWindow(title, _wndDelegate = WndProc);
        }

        public I Handle { get; }

        private I WndProc(I hWnd, uint msg, I wParam, I lParam)
        {
            if (msg == 0x8891)
            {
                if (lParam != I.Zero && wParam.ToInt64() >= 0)
                {
                    var vmId = (uint)wParam.ToInt32();
                    if (WinHandles.GetByVmId(vmId) is { } vmObj)
                    {
                        vmObj.Client = lParam;
                        _callback?.Invoke(vmObj, new EmuInitVMsg());
                    }
                }
            }
            if (msg == 0x8895)
            {
                if (wParam.ToInt32() == 1)
                {
                    if (WinHandles.GetByWinHandle(lParam) is { } vmObj)
                        _callback?.Invoke(vmObj, new VmPausedVMsg());
                }
                else if (wParam.ToInt32() == 0)
                {
                    if (WinHandles.GetByWinHandle(lParam) is { } vmObj)
                        _callback?.Invoke(vmObj, new VmResumedVMsg());
                }
            }
            if (msg == 0x8896)
            {
                if (wParam.ToInt32() == 1)
                {
                    if (WinHandles.GetByWinHandle(lParam) is { } vmObj)
                        _callback?.Invoke(vmObj, new DialogOpenedVMsg());
                }
                else if (wParam.ToInt32() == 0)
                {
                    if (WinHandles.GetByWinHandle(lParam) is { } vmObj)
                        _callback?.Invoke(vmObj, new DialogClosedVMsg());
                }
            }
            if (msg == 0x8897)
            {
                if (WinHandles.GetByWinHandle(lParam) is { } vmObj)
                    _callback?.Invoke(vmObj, new EmuShutdownVMsg());
            }
            if (msg == WM_COPYDATA)
            {
                var ds = lParam.ConvertTo<COPYDATASTRUCT>();
                var vmName = Marshal.PtrToStringAnsi(ds.lpData, ds.cbData);
                _callback?.Invoke(this, new LinkStartVMsg(vmName));
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private static I CreateMessageWindow(string title, WndProc proc)
        {
            var wndClassEx = new WNDCLASSEX
            {
                cbSize = Marshal.SizeOf<WNDCLASSEX>(),
                lpfnWndProc = proc,
                hInstance = GetModuleHandle(null),
                lpszClassName = title
            };
            var atom = RegisterClassEx(ref wndClassEx);
            if (atom == 0)
                throw new Win32Exception();
            var hWnd = CreateWindowEx(0, atom, null, 0, 0,
                0, 0, 0, I.Zero, I.Zero, I.Zero, I.Zero);
            if (hWnd == I.Zero)
                throw new Win32Exception();
            var ok = SetWindowText(hWnd, title);
            if (!ok)
                throw new Win32Exception();
            return hWnd;
        }
    }
}