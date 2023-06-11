namespace RetroBox.Fabric.Boxes
{
    public record Machine(string File, string Name, string Memory,
        string CpuLbl, string MachineLbl, string GraphicLbl, string GraphicMem,
        string Floppies, string CdRoms, string HardDrivesTxt, string SoundCard,
        string MidiChip, string NetworkTxt);
}