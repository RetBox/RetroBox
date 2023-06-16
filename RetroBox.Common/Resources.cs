using System;
using System.IO;
using System.Text;

namespace RetroBox.Common
{
    public static class Resources
    {
        public static string LoadText(string name, Type? type = null)
            => LoadStream(name, type, LoadText);

        public static byte[] LoadBytes(string name, Type? type = null)
            => LoadStream(name, type, LoadBytes);

        private static T LoadStream<T>(string name, Type? type, Func<Stream, T> func)
        {
            type ??= typeof(Resources);
            using var stream = type.Assembly.GetManifestResourceStream(name);
            return func(stream!);
        }

        private static string LoadText(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var text = reader.ReadToEnd().Trim();
            return text;
        }

        private static byte[] LoadBytes(Stream stream)
        {
            using var reader = new MemoryStream();
            stream.CopyTo(reader);
            return reader.ToArray();
        }
    }
}