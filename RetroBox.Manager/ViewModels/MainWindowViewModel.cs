using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RetroBox.API.Data;
using RetroBox.Fabric.Prebuilt;
using RetroBox.Manager.Boxes;
using IOPath = System.IO.Path;

namespace RetroBox.Manager.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<FoundExe> AllEmus { get; } = new();
        public ObservableCollection<FoundRom> AllRoms { get; } = new();

        public ObservableCollection<IMetaTemplate> AllTemplates { get; } = new();
        public ObservableCollection<MetaMachine> AllMachines { get; } = new();

        private MetaMachine? _currentMachine;

        public MetaMachine? CurrentMachine
        {
            get => _currentMachine;
            set => this.RaiseAndSetIfChanged(ref _currentMachine, value);
        }

        private string? _status = "No status and no operations are going on. Everything's calm.";

        public string? Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public MainWindowViewModel()
        {
            var i = 0;
            foreach (var state in Enum.GetValues<MachineState>())
                AllMachines.Add(new MetaMachine
                {
                    Name = $"Debug #{++i}", Description = $"It is: {state}", Status = state
                });
            CurrentMachine = AllMachines.FirstOrDefault();
        }
    }
}