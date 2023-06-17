using System;
using CliWrap;
using CliWrap.EventStream;

namespace RetroBox.Manager.CoreLogic
{
    public static class Monitor
    {
        public static IDisposable OnProcessEvent(this Command cmd, Action<Command, CommandEvent> action)
        {
            var observe = cmd.Observe();
            var sub = observe.Subscribe(
                evt => action(cmd, evt),
                ex => action(cmd, new ErrorEvent(ex)),
                () => action(cmd, new CompleteEvent()));
            return sub;
        }
    }
}