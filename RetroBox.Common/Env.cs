using System;
using System.IO;

namespace RetroBox.Common
{
    public static class Env
    {
        public static string? Resolve(string varName, string? subPath = null)
        {
            var path = Environment.GetEnvironmentVariable(varName);
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var full = subPath == null ? path : Path.Combine(path, subPath);
            path = Directory.Exists(full)
                ? Path.GetFullPath(full)
                : Directory.CreateDirectory(full).FullName;
            return path;
        }
    }
}