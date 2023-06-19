using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Unix.Native;
using RetroBox.Common.Messages;
using RetroBox.Common.Special;

#pragma warning disable CA1416

namespace RetroBox.Unix
{
    public class UnixLoop : IDisposable
    {
        private readonly PosixSignalRegistration _posix;

        public UnixLoop()
        {
            _posix = PosixSignalRegistration.Create(PosixSignal.SIGCONT, OnPosixStop);
        }

        public void Dispose()
        {
            _posix.Dispose();
        }

        ~UnixLoop()
        {
            Dispose();
        }

        public EventHandler<IVmMessage>? CallbackV;
        public EventHandler<IMgrMessage>? CallbackM;

        public void WriteStartVm(IntPtr hWnd, string vmName)
        {
            var pid = hWnd.ToInt32();
            var proc = Process.GetProcessById(pid);
            var fileMsg = GetMsgFileName(proc, pid);
            var contents = $"{string.Empty}{vmName}";
            File.WriteAllText(fileMsg, contents, Encoding.UTF8);
            Syscall.kill(pid, Signum.SIGCONT);
        }

        private void OnPosixStop(PosixSignalContext obj)
        {
            obj.Cancel = true;
            var proc = Process.GetCurrentProcess();
            var pid = Environment.ProcessId;
            var fileMsg = GetMsgFileName(proc, pid);
            if (!File.Exists(fileMsg))
                return;
            var contents = File.ReadAllText(fileMsg, Encoding.UTF8);
            File.Delete(fileMsg);
            var vmName = contents.Trim();
            CallbackM?.Invoke(this, new LinkStartVMsg(vmName));
        }

        private static string GetMsgFileName(Process proc, int pid)
        {
            var mod = proc.MainModule!;
            var fileName = mod.FileName;
            var fileDir = Path.GetDirectoryName(fileName)!;
            var fileMsg = Path.Combine(fileDir, $"p_{pid}.tmp");
            return fileMsg;
        }
    }
}