﻿using System.Collections.Generic;
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

        private string? _status;

        public string? Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public MainWindowViewModel()
        {
            Status = "No status and no operations are going on. Everything's calm.";
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