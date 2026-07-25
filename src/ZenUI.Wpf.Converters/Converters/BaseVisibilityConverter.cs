using System.Windows;

namespace ZenUI.Wpf.Converters
{
    /// <summary>
    /// 为将条件转换为 <see cref="Visibility"/> 的转换器提供公共选项。
    /// </summary>
    public abstract class BaseVisibilityConverter : BaseValueConverter
    {
        /// <summary>
        /// 获取或设置是否反转条件的可见性结果。
        /// </summary>
        public bool IsReverse { get; set; }

        /// <summary>
        /// 获取或设置条件不成立时是否返回 <see cref="Visibility.Collapsed"/>；
        /// 为 <see langword="false"/> 时返回 <see cref="Visibility.Hidden"/>。
        /// </summary>
        public bool UseCollapsed { get; set; } = true;

        /// <summary>
        /// 根据 <see cref="UseCollapsed"/> 获取不可见状态。
        /// </summary>
        /// <returns>配置为折叠时返回 <see cref="Visibility.Collapsed"/>，否则返回 <see cref="Visibility.Hidden"/>。</returns>
        protected Visibility GetInvisibleValue()
        {
            return UseCollapsed ? Visibility.Collapsed : Visibility.Hidden;
        }

        /// <summary>
        /// 将条件结果转换为可见性，并应用 <see cref="IsReverse"/> 设置。
        /// </summary>
        /// <param name="boolValue">要转换的条件结果。</param>
        /// <returns>应用当前选项后的可见性。</returns>
        protected Visibility GetFinalResult(bool boolValue)
        {
            if (IsReverse)
            {
                return boolValue ? GetInvisibleValue() : Visibility.Visible;
            }
            return boolValue ? Visibility.Visible : GetInvisibleValue();
        }
    }
}
