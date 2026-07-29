using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示供 <see cref="ToolTip"/> 和 <see cref="ZenPopover"/> 共用的气泡外观。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ZenCalloutChrome : ContentControl
    {
        private static readonly System.Type SelfType = typeof(ZenCalloutChrome);

        /// <summary>
        /// 获取或设置气泡相对于目标元素的显示方位。
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Placement"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                nameof(Placement),
                typeof(PlacementMode),
                SelfType,
                new FrameworkPropertyMetadata(PlacementMode.Top));

        /// <summary>
        /// 获取或设置气泡主体的圆角半径。
        /// </summary>
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
                SelfType,
                new FrameworkPropertyMetadata(new CornerRadius(6)));

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示指向目标元素的箭头。
        /// </summary>
        public bool ShowArrow
        {
            get { return (bool)GetValue(ShowArrowProperty); }
            set { SetValue(ShowArrowProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ShowArrow"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ShowArrowProperty =
            DependencyProperty.Register(
                nameof(ShowArrow),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(true));
    }
}
