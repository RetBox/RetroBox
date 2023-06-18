namespace RetroBox.Common.Special
{
    public sealed class LinkStartVMsg : IMgrMessage
    {
        public LinkStartVMsg(string vmName)
        {
            VmName = vmName;
        }

        public string VmName { get; }
    }
}