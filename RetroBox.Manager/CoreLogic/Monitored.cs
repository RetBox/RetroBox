using System;
using System.Collections.Generic;

namespace RetroBox.Manager.CoreLogic
{
    internal sealed class Monitored : IDisposable
    {
        public int? Pid { get; set; }

        public IDisposable? Obs { get; set; }

        public List<string> Lines { get; } = new();

        public List<Exception> Errors { get; } = new();

        public int? Exit { get; set; }

        public void Dispose()
        {
            Obs?.Dispose();
            Obs = null;
        }
    }
}