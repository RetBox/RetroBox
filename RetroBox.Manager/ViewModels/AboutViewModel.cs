using RetroBox.API.Xplat;
using RetroBox.Fabric.Meta;
using RetroBox.Fabric;

namespace RetroBox.Manager.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Dll = Infos.GetInfo<App>();
            Pc = Platforms.My.Computer;
        }

        public IPlatComputer Pc { get; }
        public DllInfo Dll { get; }
    }
}