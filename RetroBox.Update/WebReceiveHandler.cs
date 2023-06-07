using System.Net.Http;
using System.Net.Http.Handlers;

namespace RetroBox.Update
{
    public delegate void WebReceiveHandler(HttpRequestMessage req, HttpProgressEventArgs args);
}