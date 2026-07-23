using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持自定义圆角和空状态内容的数据网格控件。
    /// </summary>
    public class ZenDataGrid : DataGrid
    {
        static ZenDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(typeof(ZenDataGrid)));
        }

        /// <summary>
        /// 获取或设置数据网格的圆角。
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
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// 获取或设置数据网格没有数据时显示的内容。
        /// </summary>
        [Bindable(true)]
        public object EmptyContent
        {
            get { return GetValue(EmptyContentProperty); }
            set { SetValue(EmptyContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="EmptyContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(
                nameof(EmptyContent),
                typeof(object),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata("暂无数据"));
    }
}
