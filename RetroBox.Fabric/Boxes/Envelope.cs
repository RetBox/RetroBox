using RetroBox.Common;

namespace RetroBox.Fabric.Boxes
{
    public class Envelope
    {
        public string? File { get; set; }

        public string? Description { get; set; }

        public string? Name { get; set; }

        public void Save()
        {
            Serials.WriteJsonFile(this, InternalFile!);
        }

        internal string? InternalFile { get; set; }
    }
}