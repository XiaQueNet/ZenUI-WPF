using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 将布尔值转换为可见性。
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : BaseVisibilityConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return GetFinalResult(boolValue);
            }

            return GetInvisibleValue();
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility visibility))
            {
                return false;
            }

            bool isVisible = visibility == Visibility.Visible;
            return IsReverse ? !isVisible : isVisible;
        }
    }
}
