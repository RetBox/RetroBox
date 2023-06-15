namespace RetroBox.API.Xplat
{
    public interface IPlatFolder
    {
        string GetDefaultConfigPath();

        string GetDefaultTempPath();

        string GetDefaultHomePath();
        
        string GetCurrentExePath();
    }
}