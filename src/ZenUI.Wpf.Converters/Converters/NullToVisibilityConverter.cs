using System;
using System.Globalization;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 根据值是否为 <see langword="null"/> 或空字符串转换可见性。
    /// </summary>
    public class NullToVisibilityConverter : BaseVisibilityConverter
    {
        /// <inheritdoc />
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
