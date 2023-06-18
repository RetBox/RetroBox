using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RetroBox.Unix
{
    public static class UnixSockets
    {
        private static readonly IDictionary<string, SocketObj> Sub = new Dictionary<string, SocketObj>();

        public static void Create(string? key, string socketFile)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            var endpoint = new UnixDomainSocketEndPoint(socketFile);
            socket.Bind(endpoint);
            socket.Listen(0);

            var so = new SocketObj { Tag = key, Server = socket };
            Sub[key] = so;

            Task.Factory.StartNew(async () =>
            {
                var client = await socket.AcceptAsync();
                Sub[key].Client = client;
            });
        }

        public static void Destroy(string key)
        {
            if (!Sub.TryGetValue(key, out var so))
                return;

            Sub.Remove(key);
            so.Dispose();
        }

        public static void Write(string key, string text)
        {
            if (!Sub.TryGetValue(key, out var so) || so.Client == null)
                return;
            if (string.IsNullOrWhiteSpace(text))
                return;
            text += '\n';
            var bytes = Encoding.UTF8.GetBytes(text);
            so.Client.Send(bytes);
        }
    }
}