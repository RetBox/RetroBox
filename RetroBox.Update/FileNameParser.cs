using System;
using System.IO;
using System.Linq;
using RetroBox.API.Update;

namespace RetroBox.Update
{
    internal static class FileNameParser
    {
        public static (OsName os, OsArch[] arch, FileType type) Parse(string name, string url)
        {
            var tmp = name.Split("-");
            string lastTxt;
            if (tmp.Length >= 2)
            {
                var endPart = tmp.Last();
                var last = endPart.Split('.', 2);
                lastTxt = last.Last();
            }
            else
            {
                var last = url.Split('/').Last();
                last = last.Replace(".tar.gz", ".tgz");
                lastTxt = Path.GetExtension(last)[1..];
            }
            Enum.TryParse<FileType>(lastTxt, true, out var type);

            OsName os = default;
            OsArch[]? arch = default;

            if (name == "Source code")
            {
                os = OsName.None;
                arch = Array.Empty<OsArch>();
            }
            else if (tmp[1] == "Linux" || tmp[2] == "Linux")
            {
                os = OsName.Linux;
                if (tmp[2] == "x86")
                    arch = new[] { OsArch.x86 };
                else if (tmp[2] == "x86_64")
                    arch = new[] { OsArch.x86_64 };
                else if (tmp[3] == "arm32")
                    arch = new[] { OsArch.arm32 };
                else if (tmp[3] == "arm64")
                    arch = new[] { OsArch.arm64 };
            }
            else if (tmp[1] == "macOS")
            {
                os = OsName.MacOS;
                if (tmp[2] == "x86_64+arm64")
                    arch = new[] { OsArch.x86_64, OsArch.arm64 };
                else if (tmp[2] == "x86_64")
                    arch = new[] { OsArch.x86_64 };
            }
            else if (tmp[1] == "Windows")
            {
                os = OsName.Windows;
                if (tmp[2] == "32")
                    arch = new[] { OsArch.x86 };
                else if (tmp[2] == "64")
                    arch = new[] { OsArch.x86_64 };
            }

            if (os != default && arch != default && type != default)
                return (os, arch, type);

            throw new InvalidOperationException($"{os}:{arch}:{type} --> {name} ({url})");
        }
    }
}