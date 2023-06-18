using System.IO;
using System.Reflection;

namespace RetroBox.Common.Tools
{
    public static class Apps
    {
        public static string GetExeFolder()
        {
            var ass = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var loc = Path.GetFullPath(ass.Location);
            var dir = Path.GetDirectoryName(loc);
            return dir!;
        }

        public const string MainTitle = "RetroBox Manager";
        public const string MainSecret = $"{MainTitle} Secret";
    }
}