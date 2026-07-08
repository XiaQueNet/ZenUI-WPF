using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 默认比较式成立则显示
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ComparisonToVisibilityConverter : BaseVisibilityConverter<ComparisonToVisibilityConverter>
    {
        /// <summary>
        /// 比较方式
        /// </summary>
        public ComparisonType Comparison { get; set; } = ComparisonType.Equal;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 基础参数校验：源值或比较值为空时返回不可见
            if (value == DependencyProperty.UnsetValue || parameter == null)
            {
                return GetInvisibleValue();
            }

            try
            {
                // 1. 将参数（第二个比较值）转换为与源值相同的类型
                object compareValue = ConvertToMatchingType(parameter.ToString(), value.GetType(), culture);

                // 2. 执行核心比较逻辑（使用配置的Comparison属性）
                bool comparisonResult = CompareValues(value, compareValue, culture);

                return GetFinalResult(comparisonResult);

                // 3. 应用反转逻辑
                //bool finalResult = IsReverse ? !comparisonResult : comparisonResult;

                // 4. 返回最终的Visibility状态
                //return finalResult ? Visibility.Visible : GetInvisibleValue();
            }
            catch
            {
                // 任何异常（类型转换/比较失败）都返回不可见
                return GetInvisibleValue();
            }
        }

        /// <summary>
        /// 将字符串参数转换为与目标值相同的类型（保证比较类型一致）
        /// </summary>
        private object ConvertToMatchingType(string strValue, Type targetType, CultureInfo culture)
        {
            // 处理可空类型（如int?）
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // 特殊处理布尔值（避免字符串"True"/"False"转换失败）
            if (underlyingType == typeof(bool))
            {
                return bool.Parse(strValue);
            }

            // 通用类型转换（支持int/double/DateTime/string等）
            return System.Convert.ChangeType(strValue, underlyingType, culture);
        }

        /// <summary>
        /// 执行具体的数值/值比较逻辑
        /// </summary>
        private bool CompareValues(object value1, object value2, CultureInfo culture)
        {
            // 处理null值：只有Equal时两个null才返回true
            if (value1 == null && value2 == null)
            {
                return Comparison == ComparisonType.Equal;
            }
            if (value1 == null || value2 == null)
            {
                return false;
            }

            // 确保值实现了可比较接口
            if (!(value1 is IComparable comparable1) || !(value2 is IComparable comparable2))
            {
                throw new ArgumentException("比较的值必须实现IComparable接口（如int/string/DateTime等）");
            }

            // 根据配置的Comparison属性执行比较
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
    /// 比较方式枚举
    /// </summary>
    public enum ComparisonType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,

        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,

        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// 小于
        /// </summary>
        LessThan,

        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual
    }
}
