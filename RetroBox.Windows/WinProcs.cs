using RetroBox.Common.Messages;
using RetroBox.Common.Xplat;

namespace RetroBox.Windows
{
    internal sealed class WinProcs : CommonProc
    {
        public override void CleanUp(string tag)
        {
            throw new System.NotImplementedException(tag);
        }

        public override void Send(string tag, IVmCommand cmd)
        {
            throw new System.NotImplementedException();
        }
    }
}