using System.Net.Http;
using System.Net.Http.Handlers;

namespace RetroBox.Update
{
    internal static class HttpCentral
    {
        private static readonly HttpClient Client;

        static HttpCentral()
        {
            var core = new HttpClientHandler { AllowAutoRedirect = true };
            var handler = new ProgressMessageHandler(core);
            handler.HttpReceiveProgress += (o, args) =>
            {
                var msg = (HttpRequestMessage)o!;
                _onReceive?.Invoke(msg, args);
            };
            Client = new HttpClient(handler);
        }

        public static HttpClient GetClient(WebReceiveHandler? handler)
        {
            if (handler != null)
                _onReceive = handler;
            return Client;
        }

        private static WebReceiveHandler? _onReceive;
    }
}