using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RetroBox.Fabric;
using RetroBox.Fabric.Boxes;
using IOPath = System.IO.Path;

namespace RetroBox.Manager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<IMetaMachine> AllMachines { get; } = new();

        private IMetaMachine? _currentMachine;

        public IMetaMachine? CurrentMachine
        {
            get => _currentMachine;
            set => this.RaiseAndSetIfChanged(ref _currentMachine, value);
        }

        public MainWindowViewModel()
        {
            ReloadMachines();
        }

        private void ReloadMachines()
        {
            AllMachines.Clear();

            var folders = new List<string>
            {
                IOPath.Combine(Platforms.My.Folders.GetDefaultHomePath(), "Desktop")
            };

            foreach (var folder in folders.OrderBy(n => n))
            foreach (var machine in Machines.FindMetaMachine(folder).OrderBy(n => n.Name))
                AllMachines.Add(machine);

            CurrentMachine = AllMachines.FirstOrDefault();
        }
    }
}