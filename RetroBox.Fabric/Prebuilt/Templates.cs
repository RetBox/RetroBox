using System.Collections.Generic;
using System.IO;
using System.Linq;
using RetroBox.Common.Tools;
using RetroBox.Fabric.Boxes;

namespace RetroBox.Fabric.Prebuilt
{
    public static class Templates
    {
        public static IEnumerable<IMetaTemplate> FindMetaTemplate(string folder)
        {
            var dirs = Paths.GetDirectories(folder, "*", depth: 0).ToArray();
            foreach (var dir in dirs)
            {
                var tempName = dir.Split(Path.DirectorySeparatorChar).LastOrDefault();
                if (string.IsNullOrWhiteSpace(tempName))
                    continue;
                var tempCfg = Path.Combine(dir, Machines.BoxCfgName);
                if (!File.Exists(tempCfg))
                    continue;
                var temp = new Template(null, tempName, tempCfg);
                var tempo = new MetaTemplate(temp);
                yield return tempo;
            }
        }
    }
}