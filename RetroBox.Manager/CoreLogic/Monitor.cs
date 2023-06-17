using System;
using System.Collections.Generic;
using CliWrap;
using CliWrap.EventStream;
using RetroBox.API.Xplat;
using RetroBox.Common.Xplat;

namespace RetroBox.Manager.CoreLogic
{
    public static class Monitor
    {
        private static IDisposable OnProcessEvent(this Command cmd,
            Action<Command, string, CommandEvent> action, string tag)
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

        public static void RunThis(this Command cmd)
        {
            var id = Guid.NewGuid().ToString("N");
            GetById(id).Obs = cmd.OnProcessEvent(OnEvent, id);
        }

        private static Monitored GetById(string id)
        {
            if (!Sub.TryGetValue(id, out var value))
                Sub[id] = value = new Monitored();
            return value;
        }

        private static void OnEvent(Command cmd, string tag, CommandEvent evt)
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
    }
}