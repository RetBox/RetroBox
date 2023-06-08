using System;
using System.IO;
using System.Text;

namespace RetroBox.Common
{
    public static class Resources
    {
        public static string LoadText(string name, Type? type = null)
        {
            type ??= typeof(Resources);
            var ass = type.Assembly;
            using var stream = ass.GetManifestResourceStream(name)!;
            return LoadText(stream);
        }

        private static string LoadText(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var text = reader.ReadToEnd().Trim();
            return text;
        }
    }
}