using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSizeLib;

namespace RetroBox.Fabric.Tools
{
    public static class Strings
    {
        public static long? ToLong(this string? value)
        {
            var txt = value.ToNullIfEmpty();
            return txt == null ? null : long.Parse(txt);
        }

        public static ByteSize? AsKiloBytes(this long? value)
        {
            return value == null ? null : ByteSize.FromKiloBytes(value.Value);
        }

        public static string? ToNullIfEmpty(this string? text)
        {
            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }

        public static (double value, string unit)? AsHertzToStr(this long? speed)
        {
            if (speed == null)
                return null;
            var current = speed.Value * 1d;
            var unit = "Hz";
            if (current >= 1000)
            {
                current /= 1000.0;
                unit = "kHz";
            }
            if (current >= 1000)
            {
                current /= 1000.0;
                unit = "MHz";
            }
            return (current, unit);
        }

        public static string? FindSetting(this (string cat, string key, string val)[] all,
            string? cat = null, string? key = null)
        {
            var res = all.FindSettings(cat, key);
            var first = res.FirstOrDefault();
            return first == default ? null : first.val;
        }

        public static IEnumerable<(string cat, string key, string val)>
            FindSettings(this (string cat, string key, string val)[] all,
                string? cat = null, string? key = null)
        {
            foreach (var tuple in all)
            {
                var hasKey = key != null && tuple.key == key;
                var hasCat = cat != null && tuple.cat == cat;
                if (hasKey && hasCat)
                    yield return tuple;
                if (key == null && hasCat)
                    yield return tuple;
                if (cat == null && hasKey)
                    yield return tuple;
            }
        }

        public static string? AddPath(string? file, string folder)
        {
            if (string.IsNullOrWhiteSpace(file))
                return null;
            if (Path.IsPathRooted(file))
                return file;
            if (Path.IsPathFullyQualified(file))
                return file;
            return Path.Combine(folder, file);
        }
    }
}