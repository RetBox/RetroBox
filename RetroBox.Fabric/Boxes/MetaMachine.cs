using RetroBox.Fabric.Tools;

namespace RetroBox.Fabric.Boxes
{
    internal sealed class MetaMachine : IMetaMachine
    {
        private readonly Envelope _envelope;
        private readonly Machine _machine;

        public MetaMachine(Machine machine, Envelope envelope)
        {
            _machine = machine;
            _envelope = envelope;
        }

        public string File => _machine.File;
        public string Memory => _machine.Memory;
        public string CpuLbl => _machine.CpuLbl;
        public string MachineLbl => _machine.MachineLbl;
        public string GraphicLbl => _machine.GraphicLbl;
        public string GraphicMem => _machine.GraphicMem;
        public string Floppies => _machine.Floppies;
        public string CdRoms => _machine.CdRoms;
        public string HardDrivesTxt => _machine.HardDrivesTxt;
        public string SoundCard => _machine.SoundCard;
        public string MidiChip => _machine.MidiChip;
        public string NetworkTxt => _machine.NetworkTxt;

        public string Name
            => _envelope.Name.ToNullIfEmpty() ?? _machine.Name;

        public string Description
            => _envelope.Description.ToNullIfEmpty() ?? Machines.None;
    }
}