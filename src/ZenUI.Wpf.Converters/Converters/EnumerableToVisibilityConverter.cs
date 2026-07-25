using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 将集合是否包含数据转换为可见性。
    /// </summary>
    [ValueConversion(typeof(IEnumerable), typeof(Visibility))]
    public class EnumerableToVisibilityConverter : BaseVisibilityConverter
    {
        /// <summary>
        /// 当集合包含至少一条数据时返回可见，否则返回隐藏。
        /// </summary>
        /// <param name="value">要判断的集合对象。</param>
        /// <param name="targetType">目标绑定类型。</param>
        /// <param name="parameter">绑定参数。</param>
        /// <param name="culture">区域性信息。</param>
        /// <returns>转换后的可见性。</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable enumerable)
            {
                return GetFinalResult(HasAny(enumerable));
            }

            return GetInvisibleValue();
        }

        /// <summary>
        /// 判断集合中是否存在至少一个元素。
        /// </summary>
        /// <param name="enumerable">要检查的集合。</param>
        /// <returns>存在元素时返回true，否则返回false。</returns>
        private static bool HasAny(IEnumerable enumerable)
        {
            IEnumerator enumerator = enumerable.GetEnumerator();
            try
            {
                return enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }
    }
}
