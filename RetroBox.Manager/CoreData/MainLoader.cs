using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RetroBox.Fabric;
using RetroBox.Fabric.Boxes;
using RetroBox.Fabric.Config;
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

            model.AllRoms.Clear();
            foreach (var item in rom)
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
    }
}