using CommandLine;

namespace RetroBox.Manager.Tools
{
    public class Options
    {
        [Option('S', "start", Required = false, HelpText = "Boot up the specified VM.")]
        public string? VmName { get; set; }
    }
}