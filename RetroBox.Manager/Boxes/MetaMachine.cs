using ReactiveUI;
using RetroBox.Fabric.Boxes;
using RetroBox.Fabric.Tools;

namespace RetroBox.Manager.Boxes
{
    public class MetaMachine : ReactiveObject
    {
        #region Updating

        private void RefreshName()
        {
            Name = E?.Name.ToNullIfEmpty() ?? M?.Name;
        }

        private void RefreshCustom()
        {
            Description = E?.Description.ToNullIfEmpty() ?? Machines.None;
            var preview = E?.Preview.ToNullIfEmpty() ?? string.Empty;
            var dir = this.GetFolder();
            PreviewImg = Binary.ReadFile(Strings.AddPath(preview, dir)) ?? MetaMachines.Black;
        }

        #endregion

        #region Sub objects

        private Envelope? _envelope;

        public Envelope? E
        {
            get => _envelope;
            set
            {
                this.RaiseAndSetIfChanged(ref _envelope, value);
                RefreshName();
                RefreshCustom();
            }
        }

        private Machine? _machine;

        public Machine? M
        {
            get => _machine;
            set
            {
                this.RaiseAndSetIfChanged(ref _machine, value);
                RefreshName();
            }
        }

        #endregion

        #region Viewing

        private string? _name;

        public string? Name
        {
            get => _name;
            private set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string? _description;

        public string? Description
        {
            get => _description;
            private set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        private byte[]? _previewImg;

        public byte[]? PreviewImg
        {
            get => _previewImg;
            set => this.RaiseAndSetIfChanged(ref _previewImg, value);
        }

        #endregion

        #region Changing

        public string Preview
        {
            set
            {
                if (E is not { } envelope) return;
                envelope.Preview = value;
                envelope.Save();
                RefreshCustom();
            }
        }

        public string NameTxt
        {
            set
            {
                if (E is not { } envelope) return;
                envelope.Name = value;
                envelope.Save();
                RefreshName();
            }
        }

        public string DescriptionTxt
        {
            set
            {
                if (E is not { } envelope) return;
                envelope.Description = value;
                envelope.Save();
                RefreshCustom();
            }
        }

        #endregion
    }
}