using RetroBox.API.Data;

namespace RetroBox.API.Xplat
{
    public interface IPlatExecutable
    {
        IFileVerMeta FindExecutable(string folder);
    }
}