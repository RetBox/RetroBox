using System.IO;
using System.Text;
using RetroBox.Fabric.Boxes;

namespace RetroBox.Fabric.Prebuilt
{
    public static class Appliers
    {
        public static bool Apply(string _, string vmFolder, IMetaTemplate vmTempl)
        {
            var oldCfgName = vmTempl.ConfigFile;

            var root = Directory.CreateDirectory(vmFolder).FullName;
            var newCfgName = Path.Combine(root, Machines.BoxCfgName);

            using var fileOut = File.CreateText(newCfgName);
            var lines = File.ReadLines(oldCfgName, Encoding.UTF8);
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                fileOut.WriteLine(line);
            }

            return true;
        }
    }
}