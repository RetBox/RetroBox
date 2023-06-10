using System.Reflection;

namespace RetroBox.Fabric.Meta
{
    public static class Infos
    {
        public static DllInfo GetInfo<T>()
        {
            var ass = typeof(T).Assembly;

            var product = ass.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            var infoVer = ass.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var copy = ass.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            var config = ass.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
            var desc = ass.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

            return new DllInfo
            {
                Product = product, Version = infoVer, Config = config,
                Copyright = copy, Description = desc
            };
        }
    }
}