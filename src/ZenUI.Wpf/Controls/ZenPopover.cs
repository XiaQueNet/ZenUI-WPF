using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示由用户点击触发、显示在目标附近的轻量浮层。
    /// </summary>
    [TemplatePart(Name = PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PartTrigger, Type = typeof(ToggleButton))]
    public class ZenPopover : ContentControl
    {
        private const string PartPopup = "PART_Popup";
        private const string PartTrigger = "PART_Trigger";
        private static readonly System.Type SelfType = typeof(ZenPopover);

        static ZenPopover()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置用作浮层触发器的内容；用户单击该内容时打开或关闭浮层。
        /// 默认显示问号。
        /// </summary>
        [Bindable(true)]
        public object Trigger
        {
            get { return GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Trigger"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TriggerProperty =
            DependencyProperty.Register(
                nameof(Trigger),
                typeof(object),
                SelfType,
                new FrameworkPropertyMetadata("?"));

        /// <summary>
        /// 获取或设置用于呈现 <see cref="Trigger"/> 内容的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate TriggerTemplate
        {
            get { return (DataTemplate)GetValue(TriggerTemplateProperty); }
            set { SetValue(TriggerTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TriggerTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TriggerTemplateProperty =
            DependencyProperty.Register(
                nameof(TriggerTemplate),
                typeof(DataTemplate),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置应用于触发按钮的样式。
        /// </summary>
        [Bindable(true)]
        public Style TriggerStyle
        {
            get { return (Style)GetValue(TriggerStyleProperty); }
            set { SetValue(TriggerStyleProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TriggerStyle"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TriggerStyleProperty =
            DependencyProperty.Register(
                nameof(TriggerStyle),
                typeof(Style),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置浮层当前是否打开；该属性默认以双向方式参与数据绑定。
        /// </summary>
        [Bindable(true)]
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsOpen"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 获取或设置浮层相对于触发器的显示方位。
        /// </summary>
        [Bindable(true)]
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
        /// 获取或设置浮层在默认定位结果基础上的水平偏移量。
        /// </summary>
        [Bindable(true)]
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HorizontalOffset"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                nameof(HorizontalOffset),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// 获取或设置浮层在默认定位结果基础上的垂直偏移量。
        /// </summary>
        [Bindable(true)]
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="VerticalOffset"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                nameof(VerticalOffset),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// 获取或设置气泡箭头尖端与触发器之间的距离。
        /// </summary>
        [Bindable(true)]
        public double TargetGap
        {
            get { return (double)GetValue(TargetGapProperty); }
            set { SetValue(TargetGapProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TargetGap"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TargetGapProperty =
            DependencyProperty.Register(
                nameof(TargetGap),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(4d),
                IsValidTargetGap);

        /// <summary>
        /// 获取或设置气泡主体的圆角半径。
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
                SelfType,
                new FrameworkPropertyMetadata(new CornerRadius(6)));

        /// <summary>
        /// 获取或设置是否显示指向触发器的三角箭头。
        /// </summary>
        [Bindable(true)]
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

        /// <summary>
        /// 获取或设置气泡主体的最小宽度。
        /// </summary>
        [Bindable(true)]
        public double MinPopupWidth
        {
            get { return (double)GetValue(MinPopupWidthProperty); }
            set { SetValue(MinPopupWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="MinPopupWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MinPopupWidthProperty =
            DependencyProperty.Register(
                nameof(MinPopupWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(0d),
                IsValidPopupWidth);

        /// <summary>
        /// 获取或设置气泡主体的最大宽度。
        /// </summary>
        [Bindable(true)]
        public double MaxPopupWidth
        {
            get { return (double)GetValue(MaxPopupWidthProperty); }
            set { SetValue(MaxPopupWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="MaxPopupWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MaxPopupWidthProperty =
            DependencyProperty.Register(
                nameof(MaxPopupWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(360d),
                IsValidPopupWidth);

        private static bool IsValidPopupWidth(object value)
        {
            var width = (double)value;
            return !double.IsNaN(width) && width >= 0d;
        }

        private static bool IsValidTargetGap(object value)
        {
            var gap = (double)value;
            return !double.IsNaN(gap) &&
                !double.IsInfinity(gap) &&
                gap >= 0d;
        }

        /// <inheritdoc/>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsOpen)
            {
                IsOpen = false;
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }
    }
}
