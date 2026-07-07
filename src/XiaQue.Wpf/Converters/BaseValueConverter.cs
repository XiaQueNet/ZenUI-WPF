using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace XiaQue.Wpf.Converters
{
    /// <summary>
    /// 需要多实例、有状态的转换器
    /// </summary>
    public abstract class BaseValueConverter : MarkupExtension, IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    /// <summary>
    /// 泛型基类，单例，适用于无状态的通用转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter where T : class, new()
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static T _Instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _Instance ?? (_Instance = new T());
        }
    }
}
