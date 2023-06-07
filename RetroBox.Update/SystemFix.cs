using System.IO;

namespace RetroBox.Update
{
    public static class SystemFix
    {
        public static string Correct(string path)
        {
            var @fixed = path.Replace('/', Slash);
            return @fixed;
        }

        public static char Slash => Path.DirectorySeparatorChar;
    }
}