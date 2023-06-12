using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace RetroBox.Manager.ViewCore
{
    public sealed class BinaryImager : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] array && targetType == typeof(IBitmap))
            {
                var bitmap = new Bitmap(new MemoryStream(array));
                return bitmap;
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