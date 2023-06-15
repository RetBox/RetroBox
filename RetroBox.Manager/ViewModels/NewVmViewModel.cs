using System.Collections.ObjectModel;
using ReactiveUI;
using RetroBox.Fabric.Prebuilt;

namespace RetroBox.Manager.ViewModels
{
    public class NewVmViewModel : ViewModelBase
    {
        public ObservableCollection<IMetaTemplate> Templates { get; set; } = new();

        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _folder;

        public string Folder
        {
            get => _folder;
            set => this.RaiseAndSetIfChanged(ref _folder, value);
        }

        public NewVmViewModel()
        {
            _name = "???";
            _folder = "??";
            var t = new Template(null, "High and Good", "?");
            Templates.Add(new MetaTemplate(t));
        }
    }
}