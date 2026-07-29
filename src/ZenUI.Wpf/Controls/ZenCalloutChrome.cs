using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 为 ToolTip 和 ZenPopover 提供共用的气泡外观。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ZenCalloutChrome : ContentControl
    {
        private static readonly System.Type SelfType = typeof(ZenCalloutChrome);

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                nameof(Placement),
                typeof(PlacementMode),
                SelfType,
                new FrameworkPropertyMetadata(PlacementMode.Top));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                SelfType,
                new FrameworkPropertyMetadata(new CornerRadius(6)));

        public bool ShowArrow
        {
            get { return (bool)GetValue(ShowArrowProperty); }
            set { SetValue(ShowArrowProperty, value); }
        }

        public static readonly DependencyProperty ShowArrowProperty =
            DependencyProperty.Register(
                nameof(ShowArrow),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(true));
    }
}
