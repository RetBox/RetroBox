using System;
using System.Reflection;
using System.Text;
using RetroBox.Fabric;
using RetroBox.Fabric.Config;

Console.OutputEncoding=Encoding.UTF8;

var plat = Platforms.My;
var cfg = Configs.Default;
Console.WriteLine($"{plat} : {cfg.AppData} | {cfg.CacheRoot} | {cfg.HomePath} | {cfg.TempPath} | {cfg.EmuRoot} | {cfg.RomRoot}");

/*
var files = plat.Execs.FindExe(cfg.EmuRoot);
foreach (var file in files)
{
    Console.WriteLine($" * {JsonConvert.SerializeObject(file)}");
}

var files2 = plat.Execs.FindRom(cfg.RomRoot);
foreach (var file in files2)
{
    Console.WriteLine($" * {JsonConvert.SerializeObject(file)}");
}

var files3 = Machines.FindMachine(@"C:\Users\Hans\Desktop").OrderBy(n => n.Name);
foreach (var file in files3)
{
    Console.WriteLine($" * {JsonConvert.SerializeObject(file)}");
}
*/

Console.WriteLine();

var ass = typeof(RetroBox.Manager.App).Assembly;
var product = ass.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
var infoVer = ass.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
var copy = ass.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
var config = ass.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
var desc = ass.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

var pc = Platforms.My.Computer;
var hostName = pc.HostName;
var hostMem = pc.HostMemory;
var hostOs = pc.HostOS;
var netRuntime = pc.NetRuntime;

Console.WriteLine($"About {product}");
Console.WriteLine($"Product: {product}");
Console.WriteLine($"Description: {desc}");
Console.WriteLine($"Version: {infoVer} ({config})");
Console.WriteLine();
Console.WriteLine($"Runtime: {netRuntime}");
Console.WriteLine();
Console.WriteLine($"Host name: {hostName}");
Console.WriteLine($"Memory: {hostMem} MB");
Console.WriteLine($"System: {hostOs}");
Console.WriteLine();
Console.WriteLine(copy);

