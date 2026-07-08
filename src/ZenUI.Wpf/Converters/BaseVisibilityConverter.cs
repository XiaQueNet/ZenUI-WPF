using System.Windows;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 基础的可见性转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseVisibilityConverter<T> : BaseValueConverter where T : class, new()
    {

        /// <summary>
        /// 是否反转转换结果
        /// </summary>
        public bool IsReverse { get; set; } = false;

        /// <summary>
        /// 是否返回Collapsed
        /// </summary>
        public bool UseCollapsed { get; set; } = true;

        /// <summary>
        /// 根据UseCollapsed属性获取不可见的Visibility值
        /// </summary>
        public Visibility GetInvisibleValue()
        {
            return UseCollapsed ? Visibility.Collapsed : Visibility.Hidden;
        }

        /// <summary>
        /// 返回最终结果
        /// </summary>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public Visibility GetFinalResult(bool boolValue)
        {
            if (IsReverse)
            {
                return boolValue ? GetInvisibleValue() : Visibility.Visible;
            }
            else
            {
                return boolValue ? Visibility.Visible : GetInvisibleValue();
            }
        }

    }
}
