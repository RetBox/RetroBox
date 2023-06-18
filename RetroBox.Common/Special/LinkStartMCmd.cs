namespace RetroBox.Common.Special
{
    public sealed class LinkStartMCmd : IMgrCommand
    {
        public LinkStartMCmd(string vmName)
        {
            VmName = vmName;
        }

        public string VmName { get; }
    }
}