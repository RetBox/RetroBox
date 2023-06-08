namespace RetroBox.API.Xplat
{
    public interface IPlatform
    {
        IPlatFolder Folders { get; }
        
        IPlatExecutable Executables { get; }
    }
}