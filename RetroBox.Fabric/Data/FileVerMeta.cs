namespace RetroBox.Fabric.Data
{
    public class FileVerMeta
    {
        public int FileMajorPart { get; set; }
        public int FileMinorPart { get; set; }
        public int FileBuildPart { get; set; }
        public int FilePrivatePart { get; set; }
        public string? FileVersion { get; set; }
        public string? ReleaseId { get; set; }
    }
}