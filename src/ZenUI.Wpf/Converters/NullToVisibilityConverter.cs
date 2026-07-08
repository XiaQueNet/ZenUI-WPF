using System;
using System.Globalization;

namespace ZenUI.Wpf.Converters
{
    public class NullToVisibilityConverter : BaseVisibilityConverter<NullToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return GetFinalResult(!string.IsNullOrEmpty(value as string));
            }

            return GetFinalResult(value != null);
        }
    }
}
