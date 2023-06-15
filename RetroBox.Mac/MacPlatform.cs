using RetroBox.API.Xplat;

namespace RetroBox.Mac
{
    public sealed class MacPlatform : IPlatform
    {
        public IPlatFolder Folders { get; } = new MacFolders();

        public IPlatExec Execs { get; } = new MacExecs();

        public IPlatProc Procs { get; } = new MacProcs();

        public IPlatComputer Computer { get; } = new MacInfos();
    }
}