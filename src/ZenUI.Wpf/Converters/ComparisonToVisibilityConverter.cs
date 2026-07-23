using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 根据源值与转换参数的比较结果转换可见性。
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ComparisonToVisibilityConverter : BaseVisibilityConverter
    {
        /// <summary>
        /// 获取或设置源值与转换参数的比较方式。
        /// </summary>
        public ComparisonType Comparison { get; set; } = ComparisonType.Equal;

        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || parameter == null)
            {
                return GetInvisibleValue();
            }

            try
            {
                object compareValue = ConvertToMatchingType(parameter.ToString(), value.GetType(), culture);
                bool comparisonResult = CompareValues(value, compareValue, culture);

                return GetFinalResult(comparisonResult);
            }
            catch
            {
                // WPF 绑定转换失败时降级为不可见，避免无效输入中断布局和数据绑定。
                return GetInvisibleValue();
            }
        }

        private static object ConvertToMatchingType(string strValue, Type targetType, CultureInfo culture)
        {
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(bool))
            {
                return bool.Parse(strValue);
            }

            return System.Convert.ChangeType(strValue, underlyingType, culture);
        }

        private bool CompareValues(object value1, object value2, CultureInfo culture)
        {
            if (value1 == null && value2 == null)
            {
                return Comparison == ComparisonType.Equal;
            }
            if (value1 == null || value2 == null)
            {
                return false;
            }

            if (!(value1 is IComparable comparable1) || !(value2 is IComparable comparable2))
            {
                throw new ArgumentException("比较的值必须实现IComparable接口（如int/string/DateTime等）");
            }

            int compareResult = comparable1.CompareTo(comparable2);
            return Comparison switch
            {
                ComparisonType.Equal => compareResult == 0,
                ComparisonType.NotEqual => compareResult != 0,
                ComparisonType.GreaterThan => compareResult > 0,
                ComparisonType.GreaterThanOrEqual => compareResult >= 0,
                ComparisonType.LessThan => compareResult < 0,
                ComparisonType.LessThanOrEqual => compareResult <= 0,
                _ => false
            };
        }
    }

    /// <summary>
    /// 指定值的比较方式。
    /// </summary>
    public enum ComparisonType
    {
        /// <summary>
        /// 两个值相等。
        /// </summary>
        Equal,

        /// <summary>
        /// 两个值不相等。
        /// </summary>
        NotEqual,

        /// <summary>
        /// 源值大于转换参数。
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 源值大于或等于转换参数。
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// 源值小于转换参数。
        /// </summary>
        LessThan,

        /// <summary>
        /// 源值小于或等于转换参数。
        /// </summary>
        LessThanOrEqual
    }
}
