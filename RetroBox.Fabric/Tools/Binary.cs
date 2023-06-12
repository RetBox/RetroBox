using System.IO;

namespace RetroBox.Fabric.Tools
{
    public static class Binary
    {
        public static byte[]? ReadFile(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var bytes = File.ReadAllBytes(path);
            return bytes;
        }

        public static void WriteFile(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }
    }
}