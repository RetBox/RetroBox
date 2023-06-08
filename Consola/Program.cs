using System;
using Newtonsoft.Json;
using RetroBox.Fabric;
using RetroBox.Fabric.Config;


var plat = Platforms.My;
var cfg = Configs.Default;
Console.WriteLine(plat + " : " + cfg.AppData + " | " + cfg.CacheRoot + " | " + cfg.HomePath + " | " + cfg.TempPath + " | " + cfg.EmuRoot + " | " + cfg.RomRoot);

var files = plat.Execs.FindExe(cfg.EmuRoot);
foreach (var file in files)
{
    Console.WriteLine(" * " + JsonConvert.SerializeObject(file));
}

var files2 = plat.Execs.FindRom(cfg.RomRoot);
foreach (var file in files2)
{
    Console.WriteLine(" * " + JsonConvert.SerializeObject(file));
}



