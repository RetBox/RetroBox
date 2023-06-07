namespace RetroBox.Manager.Models
{
    internal record ExtractTask(string File, string Folder, string[] Result)
        : IProgTask;
}