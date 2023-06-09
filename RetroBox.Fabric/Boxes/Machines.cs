using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SoftCircuits.IniFileParser;

namespace RetroBox.Fabric.Boxes
{
    public static class Machines
    {
        public static IEnumerable<Machine> FindMachine(string folder)
        {
            var files = Directory.GetFiles(folder, "86box.cfg", SearchOption.AllDirectories);
            foreach (var rawFile in files)
            {
                var vmDir = Path.GetDirectoryName(rawFile);
                if (vmDir == null)
                    continue;
                var nomPts = vmDir.Split(Path.DirectorySeparatorChar).Reverse();
                var vmName = nomPts.FirstOrDefault();
                if (vmName == null)
                    continue;
                yield return ReadConfig(vmName, rawFile);
            }
        }

        private static Machine ReadConfig(string vmName, string vmFile)
        {
            var ini = new IniFile();
            ini.Load(vmFile, Encoding.UTF8);

            const string mach = "Machine";
            var memKb = ini.GetSetting(mach, "mem_size");
            var machine = ini.GetSetting(mach, "machine");
            var cpuFamily = ini.GetSetting(mach, "cpu_family");
            var cpuSpeed = ini.GetSetting(mach, "cpu_speed");
            var cpuMulti = ini.GetSetting(mach, "cpu_multi");
            var cpuNr = ini.GetSetting(mach, "cpu");

            const string vid = "Video";
            var gfxCard = ini.GetSetting(vid, "gfxcard");

            const string hd = "Hard disks";
            var hd1File = ini.GetSetting(hd, "hdd_01_fn")?.Trim();
            var hd1Size = CalcSizeMb(ini.GetSetting(hd, "hdd_01_parameters"));

            const string net = "Network";
            var netCard = ini.GetSetting(net, "net_card") ??
                          ini.GetSetting(net, "net_01_card");

            return new Machine(vmName, vmFile,
                new { memKb, machine, cpuFamily, cpuSpeed, cpuMulti, cpuNr, gfxCard, hd1File, hd1Size, netCard });
        }

        private static double? CalcSizeMb(string? text)
        {
            if (text == null)
                return null;

            var textPts = text.Split(", ");
            var sectors = int.Parse(textPts[0]);
            var heads = int.Parse(textPts[1]);
            var cylinders = int.Parse(textPts[2]);

            var mb = 512 * cylinders * heads * sectors / (1024d * 1024);
            return mb;
        }
    }
}