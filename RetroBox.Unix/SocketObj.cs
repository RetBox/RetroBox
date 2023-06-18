using System;
using System.Net.Sockets;

namespace RetroBox.Unix
{
    public sealed class SocketObj : IDisposable
    {
        public string? Tag { get; set; } 

        public Socket? Server { get; set; }

        public Socket? Client { get; set; }

        public void Dispose()
        {
            Server?.Dispose();
            Server = null;
            Client?.Dispose();
            Client = null;
        }
    }
}