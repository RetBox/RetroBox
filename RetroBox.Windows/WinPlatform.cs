using RetroBox.API.Xplat;

namespace RetroBox.Windows
{
    public sealed class WinPlatform : IPlatform
    {
        public IPlatFolder Folders { get; } = new WinFolders();

        public IPlatExec Execs { get; } = new WinExecs();

        public IPlatComputer Computer { get; } = new WinInfos();
    }
}