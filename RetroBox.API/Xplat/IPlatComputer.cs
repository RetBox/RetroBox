namespace RetroBox.API.Xplat
{
    public interface IPlatComputer
    {
        string HostName { get; }

        long HostMemory { get; }

        string HostOS { get; }

        string NetRuntime { get; }
    }
}