using ReactiveUI;

namespace RetroBox.Manager.ViewModels
{
    public class ParticleViewModel : ViewModelBase
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        private string _key;

        public string Key
        {
            get => _key;
            set => this.RaiseAndSetIfChanged(ref _key, value);
        }

        private string _val;

        public string Val
        {
            get => _val;
            set => this.RaiseAndSetIfChanged(ref _val, value);
        }

        public ParticleViewModel()
        {
            _title = "??? ??? ??? ???";
            _key = "??? ??? ???";
            _val = "!!! !!! !!!";
        }
    }
}