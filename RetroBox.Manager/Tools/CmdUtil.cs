using System;
using System.Text;
using CommandLine;

namespace RetroBox.Manager.Tools
{
    internal static class CmdUtil
    {
        public static Options Parse(string[] args)
        {
            var parser = Parser.Default;
            var parsed = parser.ParseArguments<Options>(args);
            if (parsed.Value is { } opt)
            {
                return opt;
            }
            var lines = new StringBuilder();
            foreach (var error in parsed.Errors)
            {
                var line = error.ToString();
                switch (error)
                {
                    case UnknownOptionError ue:
                        line = $"Unknown option: {ue.Token}";
                        break;
                    case MissingRequiredOptionError me:
                        line = $"Missing required option: {me.NameInfo.NameText}";
                        break;
                }
                lines.AppendLine(line);
            }
            var errText = lines.ToString();
            throw new InvalidOperationException(errText);
        }
    }
}