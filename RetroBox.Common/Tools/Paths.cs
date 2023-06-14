using System;
using System.Collections.Generic;
using System.IO;

namespace RetroBox.Common.Tools
{
    public static class Paths
    {
        private static IEnumerable<string> GetFilesOrDirs(string root, string term, bool getFiles, int depth)
        {
            if (!Directory.Exists(root))
                return Array.Empty<string>();

            var opts = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                MaxRecursionDepth = depth,
                RecurseSubdirectories = depth > 1,
                MatchCasing = MatchCasing.PlatformDefault,
                MatchType = MatchType.Simple,
                ReturnSpecialDirectories = false
            };

            var res = getFiles
                ? Directory.EnumerateFiles(root, term, opts)
                : Directory.EnumerateDirectories(root, term, opts);
            return res;
        }

        public static IEnumerable<string> GetFiles(string root, string term, int depth = 3)
            => GetFilesOrDirs(root, term, getFiles: true, depth);

        public static IEnumerable<string> GetDirectories(string root, string term, int depth = 3)
            => GetFilesOrDirs(root, term, getFiles: false, depth);
    }
}