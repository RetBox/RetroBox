namespace RetroBox.API.Xplat
{
    public interface IPlatform
    {
        IPlatFolder Folders { get; }

        IPlatExec Execs { get; }

        IPlatComputer Computer { get; }
    }
}