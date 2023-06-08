using RetroBox.API.Xplat;

namespace RetroBox.Linux
{
    public sealed class LinuxPlatform : IPlatform
    {
        public IPlatFolder Folders { get; } = new LinuxFolders();

        public IPlatExec Execs { get; } = new LinuxExecs();
    }
}