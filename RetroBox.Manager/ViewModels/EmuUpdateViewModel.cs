using System.Collections.ObjectModel;
using RetroBox.API.Update;

namespace RetroBox.Manager.ViewModels
{
    public class EmuUpdateViewModel : ViewModelBase
    {
        public ObservableCollection<Release> Emus { get; } = new();

        public ObservableCollection<Release> Roms { get; } = new();
    }
}