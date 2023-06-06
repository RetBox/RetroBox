namespace RetroBox.API
{
    public interface IPlatform
    {
        string GetDefaultConfigPath();

        string GetDefaultTempPath();

        string GetDefaultHomePath();
    }
}