using System.Collections.Generic;
using RetroBox.Common;

namespace RetroBox.Fabric.Data
{
    public static class Internals
    {
        private static readonly Dictionary<string, IDictionary<string, EmulatedInfo>> _inter;

        static Internals()
        {
            var jsonText = Resources.LoadText("86BoxInternals.json", typeof(Internals));
            _inter = Serials.ReadJson<Dictionary<string, IDictionary<string, EmulatedInfo>>>(jsonText)!;
        }

        public static bool TryFindByName(string? name, out string? category, out EmulatedInfo? info)
        {
            if (name == null)
            {
                category = default;
                info = default;
                return false;
            }
            foreach (var pair in _inter)
            {
                var catDict = pair.Value;
                if (!catDict.TryGetValue(name, out var found))
                    continue;
                category = pair.Key;
                info = found;
                return true;
            }
            category = default;
            info = default;
            return false;
        }
    }
}