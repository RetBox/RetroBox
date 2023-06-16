using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ByteSizeLib;
using RetroBox.Common;
using RetroBox.Common.Tools;
using RetroBox.Fabric.Data;
using RetroBox.Fabric.Tools;
using SoftCircuits.IniFileParser;

namespace RetroBox.Fabric.Boxes
{
    public static class Machines
    {
        public const string BoxCfgName = "86box.cfg";

        public static IEnumerable<Machine> FindMachine(string folder)
        {
            var files = Paths.GetFiles(folder, BoxCfgName);
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

        public const string None = "None";

        private static Machine ReadConfig(string vmName, string vmFile)
        {
            var ini = new IniFile();
            ini.Load(vmFile, Encoding.UTF8);

            var all = ini.GetSections().SelectMany(s =>
                    ini.GetSectionSettings(s).Select(t => (s, t.Name!, t.Value!)))
                .ToArray();

            const string mach = "Machine";

            var memKb = all.FindSetting(mach, "mem_size")
                ?.Trim().ToLong().AsKiloBytes();
            var memKbTxt = memKb?.ToString() ?? None;

            var machine = all.FindSetting(mach, "machine")
                ?.Trim();
            Internals.TryFindByName(machine, out _, out var mInfo);
            var machineName = mInfo?.Label ?? None;

            var cpuFamily = all.FindSetting(mach, "cpu_family")
                ?.Trim();
            Internals.TryFindByName(cpuFamily, out _, out var cInfo);
            var familyName = cInfo?.Label ?? None;

            var cpuSpeed = all.FindSetting(mach, "cpu_speed")
                ?.Trim().ToLong().AsHertzToStr();
            var speedTxt = cpuSpeed == null ? None : $"{cpuSpeed.Value.value:0.##} {cpuSpeed.Value.unit}";
            var cpuLbl = $"{familyName} {speedTxt}";

            const string vid = "Video";
            var gfxCard = all.FindSetting(vid, "gfxcard")?.Trim();
            Internals.TryFindByName(gfxCard, out _, out var gInfo);
            var gfxLbl = gInfo?.Label ?? None;

            var gfxMemKb = all.FindSetting(vid, "memory")?.Trim().ToLong() ?? 0;
            var otMemKb = all.FindSettings(key: "memory")
                .Select(m => m.val.ToLong()).Sum() ?? 0;
            var fbMemMb = all.FindSettings(key: "framebuffer_memory")
                .Concat(all.FindSettings(key: "texture_memory"))
                .Select(m => m.val.ToLong()).Sum() ?? 0;
            var gfxAllMem = (ByteSize.FromKiloBytes(gfxMemKb)
                             + ByteSize.FromKiloBytes(otMemKb)
                             + ByteSize.FromMegaBytes(fbMemMb)).ToString();
            if (gfxAllMem == "0 b")
                gfxAllMem = None;

            var floppies = Enumerable.Range(1, 4).Select(i => $"fdd_0{i}_type")
                .SelectMany(f => all.FindSettings(key: f))
                .OrderBy(f => f)
                .Where(f => !f.val.Trim().Equals("none"))
                .Select(f =>
                {
                    var intName = f.val.Trim();
                    Internals.TryFindByName(intName, out _, out var fInfo);
                    return fInfo?.Label ?? None;
                }).ToArray();
            var floppyTxt = string.Join(", ", floppies);
            if (floppyTxt.Length == 0)
                floppyTxt = None;

            var cds = Enumerable.Range(1, 4).Select(i => $"cdrom_0{i}_parameters")
                .SelectMany(f => all.FindSettings(key: f))
                .OrderBy(f => f)
                .Select(f =>
                {
                    var cdBus = f.val.Trim().Split(',', 2).Last().Trim();
                    var cdSpeed = all.FindSetting(f.cat, f.key.Replace("parameters", "speed"))?.Trim().ToLong() ?? 1;
                    var cdChId = ExtractChannel(cdBus, f.cat, f.key, all);
                    return (f.key, cdSpeed, cdChId);
                })
                .ToArray();
            var cdromTxt = string.Join(", ", cds.Select(c => $"{c.cdSpeed}x CD ({c.cdChId})"));
            if (cdromTxt.Length == 0)
                cdromTxt = None;

            const string hd = "Hard disks";

            var hds = Enumerable.Range(1, 6).Select(i => $"hdd_0{i}_fn")
                .SelectMany(f => all.FindSettings(hd, key: f))
                .OrderBy(f => f)
                .Select(f =>
                {
                    var hdPkey = f.key.Replace("fn", "parameters");
                    var hdF = all.FindSetting(f.cat, hdPkey)?.Trim();
                    var hdBus = hdF?.Split(',', 5).Last().Trim()!;
                    var hdC = ExtractChannel(hdBus, f.cat, hdPkey, all);
                    var hdS = CalcSizeMb(hdF);
                    var hFile = f.val.Split('\\', '/').Last().Trim();
                    return (f.key, hdS, hdC, hFile);
                })
                .ToArray();
            var hdLines = hds.Select(c => $"{c.hFile} ({c.hdS:0.##} MB)").ToArray();
            if (hdLines.Length == 0)
                hdLines = new[] { None };
            var hdTxt = string.Join(Environment.NewLine, hdLines);

            const string snd = "Sound";

            var sndCard = all.FindSetting(snd, "sndcard")?.Trim();
            Internals.TryFindByName(sndCard, out _, out var sInfo);
            var sndCardTxt = sInfo?.Label ?? None;

            var midCard = all.FindSetting(snd, "midi_device")?.Trim();
            Internals.TryFindByName(midCard, out _, out var mmInfo);
            var mmCardTxt = mmInfo?.Label ?? None;

            const string net = "Network";

            var nts = new[] { "net_card" }.Concat(Enumerable.Range(1, 4).Select(i => $"net_0{i}_card"))
                .SelectMany(f => all.FindSettings(net, key: f))
                .OrderBy(f => f)
                .Select(f =>
                {
                    var intName = f.val.Trim();
                    Internals.TryFindByName(intName, out _, out var fInfo);
                    var intLbl = fInfo?.Label ?? None;
                    var intType = (all.FindSetting(net, f.key.Replace("card", "net_type"))
                                   ?? all.FindSetting(net, f.key.Replace("card", "type")))?.Trim()!;
                    return (f.key, intLbl, intType);
                })
                .ToArray();
            var netLines = nts.Select(c => $"{c.intLbl} ({c.intType})").ToArray();
            if (netLines.Length == 0)
                netLines = new[] { None };
            var netTxt = string.Join(Environment.NewLine, netLines);

            return new Machine(vmFile, vmName, memKbTxt, cpuLbl, machineName,
                gfxLbl, gfxAllMem, floppyTxt, cdromTxt,
                hdTxt, sndCardTxt, mmCardTxt, netTxt);
        }

        private static string ExtractChannel(string cdBus, string cat, string key, (string s, string, string)[] all)
        {
            var cdChId = string.Empty;
            if (cdBus == "scsi")
            {
                var cdScsi = all.FindSetting(cat, key.Replace("parameters", "scsi_location"))?.Trim();
                if (cdScsi != null)
                    cdChId = cdScsi;
                else
                    cdChId = $"0:0{all.FindSetting(cat, key.Replace("parameters", "scsi_id"))?.Trim()}";
            }
            else if (cdBus is "atapi" or "ide")
            {
                cdChId = all.FindSetting(cat, key.Replace("parameters", "ide_channel"))?.Trim();
            }

            return cdChId ?? string.Empty;
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

        public static IEnumerable<IMetaMachine> FindMetaMachine(string folder)
        {
            var machines = FindMachine(folder);
            var envelopes = machines.Select(Envelopes.GetEnvelope);
            return envelopes;
        }

        public static byte[] Black { get; } = Resources.LoadBytes("black.png");
    }
}