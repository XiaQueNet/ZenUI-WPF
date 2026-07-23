using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZenUI.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : BaseVisibilityConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return GetFinalResult(boolValue);
            }

            return GetInvisibleValue();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility visibility))
            {
                return false;
            }

            bool isVisible = visibility == Visibility.Visible;
            // 反向转换也对齐IsReverse规则：Visible对应的值随IsReverse反转
            return IsReverse ? !isVisible : isVisible;
        }
    }
}
