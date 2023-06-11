using System.IO;
using System.Text;
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

        public static T? ReadJsonFile<T>(string file)
        {
            var json = File.ReadAllText(file, Encoding.UTF8);
            return ReadJson<T>(json);
        }

        public static string WriteJson<T>(T obj)
        {
            var res = JsonConvert.SerializeObject(obj, Config);
            return res;
        }

        public static void WriteJsonFile<T>(T obj, string file)
        {
            var json = WriteJson(obj);
            File.WriteAllText(file, json, Encoding.UTF8);
        }
    }
}