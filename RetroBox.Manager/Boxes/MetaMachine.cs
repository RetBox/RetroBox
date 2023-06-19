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

        private void RefreshStatus()
        {
            var status = Status ?? default(MachineState);
            StatusImg = status.GetImage();
            StatusTxt = status.GetText();
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
                Status = MachineState.Stopped;
            }
        }

        #endregion

        #region Only in-memory

        private MachineState? _status;

        public MachineState? Status
        {
            get => _status;
            set
            {
                this.RaiseAndSetIfChanged(ref _status, value);
                RefreshStatus();
            }
        }

        private string? _statusTxt;

        public string? StatusTxt
        {
            get => _statusTxt;
            set => this.RaiseAndSetIfChanged(ref _statusTxt, value);
        }

        private byte[]? _statusImg;

        public byte[]? StatusImg
        {
            get => _statusImg;
            set => this.RaiseAndSetIfChanged(ref _statusImg, value);
        }

        #endregion

        #region Viewing

        private string? _name;

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string? _description;

        public string? Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
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

        public (string emuId, string romId) LastEmuUsed
        {
            set
            {
                if (E is not { } envelope) return;
                envelope.IdEmu = value.emuId;
                envelope.IdRom = value.romId;
                envelope.Save();
            }
        }

        #endregion

        public string? Tag { get; set; }
    }
}