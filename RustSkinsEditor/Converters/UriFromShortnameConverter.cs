using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace RustSkinsEditor.Converters
{
    public class UriFromShortnameConverter : IValueConverter
    {
        public static Dictionary<string, Uri> pathsCache = new Dictionary<string, Uri>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var shortname = value as string;

            if (shortname != null)
            {
                Uri result;

                if (pathsCache.TryGetValue(shortname, out result)) return result;

                string partialpath = "";

                if (parameter != null && parameter.ToString() != null) { partialpath = parameter.ToString(); }

                string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string imagepath = Path.Combine(debugpath, partialpath, shortname);

                if (!imagepath.EndsWith(".png") && !imagepath.EndsWith(".jpg") && !imagepath.EndsWith(".jpeg"))
                {
                    imagepath = imagepath + ".png";
                }

                if (File.Exists(imagepath))
                {
                    result = new Uri(imagepath, UriKind.RelativeOrAbsolute);

                    if (!pathsCache.ContainsKey(shortname))
                        pathsCache.Add(shortname, result);

                    return result;
                }
                else
                {
                    result = new Uri("/RustSkinsEditor;component/Assets/unavailable.png", UriKind.Relative);

                    if (!pathsCache.ContainsKey(shortname))
                        pathsCache.Add(shortname, result);

                    return result;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
