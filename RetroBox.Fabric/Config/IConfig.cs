namespace RetroBox.Fabric.Config
{
    public interface IConfig
    {
        string HomePath { get; }

        string TempPath { get; }

        string AppData { get; }

        string EmuRoot { get; }

        string RomRoot { get; }

        string CacheRoot { get; }
    }
}