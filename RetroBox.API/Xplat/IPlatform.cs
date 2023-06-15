namespace RetroBox.API.Xplat
{
    public interface IPlatform
    {
        IPlatFolder Folders { get; }

        IPlatExec Execs { get; }

        IPlatProc Procs { get; }

        IPlatComputer Computer { get; }
    }
}