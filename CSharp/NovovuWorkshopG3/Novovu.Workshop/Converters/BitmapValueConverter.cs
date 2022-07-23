using Avalonia;
using Avalonia.Markup;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Diagnostics;
using Avalonia.Media;

namespace Novovu.Workshop.Converters
{
    public class BitmapValueConverter : IValueConverter
    {
        public static BitmapValueConverter Instance = new BitmapValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && targetType == typeof(IImage))
            {
                var uri = new Uri((string)value, UriKind.RelativeOrAbsolute);
                var scheme = uri.IsAbsoluteUri ? uri.Scheme : "file";

                switch (scheme)
                {
                    case "file":
                        return new Bitmap((string)value);

                    default:
                        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                        return new Bitmap(assets.Open(uri));
                }
            }
            return null;
            //throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}