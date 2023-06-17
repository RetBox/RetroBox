using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace RetroBox.Manager.ViewCore
{
    public sealed class EnumComparer : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Enum eNum && targetType == typeof(bool) && parameter is string args)
            {
                var vT = value.GetType();
                var allowed = args.Split('|').Select(arg =>
                    Enum.TryParse(vT, arg, true, out var p) ? p : null);
                foreach (var allow in allowed)
                    if (eNum.Equals(allow))
                        return true;

                return false;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Nothing to do
            return null;
        }
    }
}