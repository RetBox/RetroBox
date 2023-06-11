namespace RetroBox.Fabric.Boxes
{
    public interface IMetaMachine
    {
        string File { get; }
        string Name { get; }
        string Memory { get; }
        string CpuLbl { get; }
        string MachineLbl { get; }
        string GraphicLbl { get; }
        string GraphicMem { get; }
        string Floppies { get; }
        string CdRoms { get; }
        string HardDrivesTxt { get; }
        string SoundCard { get; }
        string MidiChip { get; }
        string NetworkTxt { get; }
    }
}