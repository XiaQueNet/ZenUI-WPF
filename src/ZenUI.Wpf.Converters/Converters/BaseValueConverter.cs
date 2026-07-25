using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 为需要独立实例和可配置状态的值转换器提供基类。
    /// </summary>
    public abstract class BaseValueConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <inheritdoc />
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
