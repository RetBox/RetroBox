namespace RetroBox.Common.Messages
{
    public sealed class LinkStartVMsg : IVmMessage
    {
        public LinkStartVMsg(string vmName)
        {
            VmName = vmName;
        }

        public string VmName { get; }
    }
}