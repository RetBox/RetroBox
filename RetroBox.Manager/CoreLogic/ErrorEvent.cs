using System;
using CliWrap.EventStream;

namespace RetroBox.Manager.CoreLogic
{
    public sealed class ErrorEvent : CommandEvent
    {
        public ErrorEvent(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; }
    }
}