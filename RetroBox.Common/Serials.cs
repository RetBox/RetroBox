using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RetroBox.Common
{
    public static class Serials
    {
        private static readonly JsonSerializerSettings Config = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new StringEnumConverter() }
        };

        public static T? ReadJson<T>(string text)
        {
            var res = JsonConvert.DeserializeObject<T>(text, Config);
            return res;
        }
    }
}