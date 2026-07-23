using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 需要多实例、有状态的转换器
    /// </summary>
    public abstract class BaseValueConverter : MarkupExtension, IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
