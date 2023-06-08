namespace RetroBox.API.Data
{
    public interface IFileVerMeta
    {
        int FileMajorPart { get; set; }
        int FileMinorPart { get; set; }
        int FileBuildPart { get; set; }
        int FilePrivatePart { get; set; }
        string? FileVersion { get; set; }
        string? ReleaseId { get; set; }
    }
}