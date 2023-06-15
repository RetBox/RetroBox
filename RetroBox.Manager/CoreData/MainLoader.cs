using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RetroBox.Fabric;
using RetroBox.Fabric.Boxes;
using RetroBox.Fabric.Config;
using RetroBox.Fabric.Prebuilt;
using RetroBox.Manager.ViewModels;

namespace RetroBox.Manager.CoreData
{
    internal static class MainLoader
    {
        public static void SearchSoftware(this MainWindowViewModel model, CancellationToken token)
        {
            var config = Configs.Default;
            var home = config.HomePath;

            var plat = Platforms.My;
            plat.Execs.FindSystemic(home, out var exe, out var rom, token);

            model.AllEmus.Clear();
            foreach (var item in exe)
            {
                token.ThrowIfCancellationRequested();
                model.AllEmus.Add(item);
            }

            var emuRoot = config.EmuRoot;
            foreach (var item in plat.Execs.FindExe(emuRoot))
            {
                token.ThrowIfCancellationRequested();
                model.AllEmus.Add(item);
            }

            model.AllRoms.Clear();
            foreach (var item in rom)
            {
                token.ThrowIfCancellationRequested();
                model.AllRoms.Add(item);
            }

            var romRoot = config.RomRoot;
            foreach (var item in plat.Execs.FindRom(romRoot))
            {
                token.ThrowIfCancellationRequested();
                model.AllRoms.Add(item);
            }
        }

        public static void SearchMachines(this MainWindowViewModel model, CancellationToken token)
        {
            var config = Configs.Default;
            var mach = config.MachineRoot;

            model.AllMachines.Clear();
            var folders = new List<string> { mach };

            foreach (var folder in folders.OrderBy(n => n))
            {
                token.ThrowIfCancellationRequested();
                foreach (var machine in Machines.FindMetaMachine(folder).OrderBy(n => n.Name))
                {
                    token.ThrowIfCancellationRequested();
                    model.AllMachines.Add(machine);
                }
            }

            model.CurrentMachine = model.AllMachines.FirstOrDefault();
        }

        public static void SearchTemplates(this MainWindowViewModel model, CancellationToken token)
        {
            var config = Configs.Default;
            var temp = config.TemplateRoot;

            model.AllTemplates.Clear();
            var folders = new List<string> { temp };

            foreach (var folder in folders.OrderBy(n => n))
            {
                token.ThrowIfCancellationRequested();
                foreach (var template in Templates.FindMetaTemplate(folder).OrderBy(n => n.Name))
                {
                    token.ThrowIfCancellationRequested();
                    model.AllTemplates.Add(template);
                }
            }
        }
    }
}