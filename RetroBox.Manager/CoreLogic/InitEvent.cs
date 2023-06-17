using System;
using CliWrap.EventStream;

namespace RetroBox.Manager.CoreLogic
{
    public sealed class InitEvent : CommandEvent
    {
        public InitEvent(IDisposable dis)
        {
            Disposable = dis;
        }

        public IDisposable Disposable { get; }
    }
}