using System;
using System.Collections.Generic;
using System.Diagnostics;
using CliWrap;
using CliWrap.EventStream;
using RetroBox.API.Xplat;
using RetroBox.Common.Xplat;

namespace RetroBox.Manager.CoreLogic
{
    public static class Monitor
    {
        private static IDisposable OnProcessEvent(this Command cmd, MonitorHandler action, string tag)
        {
            var observe = cmd.Observe();
            var sub = observe.Subscribe(
                evt => action(cmd, tag, evt),
                ex => action(cmd, tag, new ErrorEvent(ex)),
                () => action(cmd, tag, new CompleteEvent()));
            return sub;
        }

        public static CommonProc GetProcs(this IPlatform plat) => (CommonProc)plat.Procs;

        private static readonly IDictionary<string, Monitored> Sub = new Dictionary<string, Monitored>();

        public static string GenerateId() => Guid.NewGuid().ToString("N");

        public static void RunThis(this Command cmd, string id, MonitorHandler? extra = null)
        {
            void EventCb(object c, string t, CommandEvent e)
            {
                var current = Sub.TryGetValue(t, out var v1) ? v1 : null;
                OnEvent(c, t, e);
                current = Sub.TryGetValue(t, out var v2) ? v2 : current;
                extra?.Invoke(current!, t, e);
            }

            var sub = cmd.OnProcessEvent(EventCb, id);
            EventCb(cmd, id, new InitEvent(sub));
        }

        private static Monitored GetById(string id)
        {
            if (!Sub.TryGetValue(id, out var value))
                Sub[id] = value = new Monitored();
            return value;
        }

        private static void OnEvent(object _, string tag, CommandEvent evt)
        {
            if (evt is CompleteEvent)
            {
                var reg = GetById(tag);
                Sub.Remove(tag);
                reg.Dispose();
                return;
            }
            if (evt is StartedCommandEvent se)
            {
                GetById(tag).Pid = se.ProcessId;
                return;
            }
            if (evt is InitEvent ie)
            {
                GetById(tag).Obs = ie.Disposable;
                return;
            }
            if (evt is ExitedCommandEvent ee)
            {
                GetById(tag).Exit = ee.ExitCode;
                return;
            }
            if (evt is ErrorEvent xe)
            {
                GetById(tag).Errors.Add(xe.Error);
                return;
            }
            var text = (evt as StandardOutputCommandEvent)?.Text
                       ?? (evt as StandardErrorCommandEvent)?.Text;
            if (text == null)
                return;
            GetById(tag).Lines.Add(text);
        }

        public static Process? FindProcess(string? tag)
        {
            if (tag == null || !Sub.TryGetValue(tag, out var cur) || cur.Pid is not { } pid)
                return null;
            var proc = Process.GetProcessById(pid);
            return proc;
        }
    }
}