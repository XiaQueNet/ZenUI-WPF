using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观并保留 WPF 原生选择行为的列表框。
    /// </summary>
    public class ZenListBox : ListBox
    {
        static ZenListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenListBox),
                new FrameworkPropertyMetadata(typeof(ZenListBox)));
        }

        /// <summary>
        /// 获取或设置列表框的圆角。
        /// </summary>
        [Bindable(true)]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CornerRadius"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(ZenListBox),
                new FrameworkPropertyMetadata(new CornerRadius(8)));
    }
}
