using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static RetroBox.Windows.Core.WinImports;

namespace RetroBox.Windows.Core
{
    internal static class WinHandles
    {
        private static readonly IDictionary<string, HandleObj> Sub = new Dictionary<string, HandleObj>();

        public static void Create(string? key, uint id, IntPtr hWnd)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            var so = new HandleObj { Tag = key, VmId = id, HWnd = hWnd };
            Sub[key] = so;
        }

        public static void Destroy(string key)
        {
            if (!Sub.TryGetValue(key, out var so))
                return;

            Sub.Remove(key);
            so.Dispose();
        }

        public static void Write(string key, (int, IntPtr, IntPtr)? pair)
        {
            if (!Sub.TryGetValue(key, out var so) || so.Client == null)
                return;
            if (pair == null)
                return;
            var client = so.Client.Value;
            var (wMsg, wParam, lParam) = pair.Value;
            PostMessage(client, wMsg, wParam, lParam);
        }

        public static void Write(IntPtr hWnd, string text)
        {
            COPYDATASTRUCT cds;
            cds.dwData = IntPtr.Zero;
            cds.lpData = Marshal.StringToHGlobalAnsi(text);
            cds.cbData = text.Length;
            SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds);
        }

        internal static uint GetTempId(string vmPath)
        {
            var tempId = vmPath.GetHashCode();
            uint id;
            if (tempId < 0)
                id = (uint)(tempId + int.MaxValue);
            else
                id = (uint)tempId;
            return id;
        }

        internal static HandleObj? GetByVmId(uint vmId)
        {
            var vmObj = Sub.Values.FirstOrDefault(v => v.VmId == vmId);
            return vmObj;
        }

        public static HandleObj? GetByWinHandle(IntPtr hWnd)
        {
            var vmObj = Sub.Values.FirstOrDefault(v => v.Client == hWnd);
            return vmObj;
        }

        public static T? ConvertTo<T>(this IntPtr pointer)
        {
            var type = typeof(T);
            var raw = Marshal.PtrToStructure(pointer, type);
            return raw is T obj ? obj : default;
        }
    }
}