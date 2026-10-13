using System;
using System.Windows;

namespace ZenUI.Wpf.Controls
{
    /// <summary>设置遮罩内对话框面板的目标尺寸。</summary>
    public static class ZenOverlayDialogWindowOptions
    {
        /// <summary>获取对话框面板的目标宽度。</summary>
        public static double GetDialogWidth(DependencyObject element) => (double)element.GetValue(DialogWidthProperty);

        /// <summary>设置对话框面板的目标宽度；空间不足时面板会缩小。</summary>
        public static void SetDialogWidth(DependencyObject element, double value) => element.SetValue(DialogWidthProperty, value);

        /// <summary>标识对话框面板目标宽度的附加属性。</summary>
        public static readonly DependencyProperty DialogWidthProperty = DependencyProperty.RegisterAttached(
            "DialogWidth", typeof(double), typeof(ZenOverlayDialogWindowOptions),
            new FrameworkPropertyMetadata(600d), IsValidDimension);

        /// <summary>获取对话框面板的目标高度。</summary>
        public static double GetDialogHeight(DependencyObject element) => (double)element.GetValue(DialogHeightProperty);

        /// <summary>设置对话框面板的目标高度；空间不足时面板会缩小。</summary>
        public static void SetDialogHeight(DependencyObject element, double value) => element.SetValue(DialogHeightProperty, value);

        /// <summary>标识对话框面板目标高度的附加属性。</summary>
        public static readonly DependencyProperty DialogHeightProperty = DependencyProperty.RegisterAttached(
            "DialogHeight", typeof(double), typeof(ZenOverlayDialogWindowOptions),
            new FrameworkPropertyMetadata(360d), IsValidDimension);

        private static bool IsValidDimension(object value)
        {
            var dimension = (double)value;
            return dimension > 0 && !double.IsInfinity(dimension) && !double.IsNaN(dimension);
        }
    }
}
