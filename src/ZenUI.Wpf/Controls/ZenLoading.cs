using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示在内容上方呈现不确定进度状态的加载容器。
    /// </summary>
    public class ZenLoading : ContentControl
    {
        private static readonly System.Type SelfType = typeof(ZenLoading);

        static ZenLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
#if ZENUI_LIVE_REGIONS
            AutomationProperties.LiveSettingProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(AutomationLiveSetting.Polite));
#endif
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示加载状态。
        /// </summary>
        [Bindable(true)]
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsLoading"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                nameof(IsLoading),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(false, OnIsLoadingChanged));

        /// <summary>
        /// 获取或设置加载指示器下方显示的说明文字。
        /// </summary>
        [Bindable(true)]
        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="LoadingText"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register(
                nameof(LoadingText),
                typeof(string),
                SelfType,
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置圆形加载指示器的边长。该值必须为大于零的有限值。
        /// </summary>
        [Bindable(true)]
        public double IndicatorSize
        {
            get { return (double)GetValue(IndicatorSizeProperty); }
            set { SetValue(IndicatorSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IndicatorSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IndicatorSizeProperty =
            DependencyProperty.Register(
                nameof(IndicatorSize),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(
                    24d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidIndicatorSize);

        /// <summary>
        /// 获取或设置加载指示器与说明文字的排列方向。
        /// </summary>
        [Bindable(true)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Orientation"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                SelfType,
                new FrameworkPropertyMetadata(
                    Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidOrientation);

        /// <summary>
        /// 获取或设置加载层的背景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush OverlayBackground
        {
            get { return (Brush)GetValue(OverlayBackgroundProperty); }
            set { SetValue(OverlayBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="OverlayBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OverlayBackgroundProperty =
            DependencyProperty.Register(
                nameof(OverlayBackground),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置一个值，该值指示显示加载状态时是否阻止用户与内容交互。
        /// </summary>
        [Bindable(true)]
        public bool IsContentInteractionBlocked
        {
            get { return (bool)GetValue(IsContentInteractionBlockedProperty); }
            set { SetValue(IsContentInteractionBlockedProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsContentInteractionBlocked"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsContentInteractionBlockedProperty =
            DependencyProperty.Register(
                nameof(IsContentInteractionBlocked),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(true));

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenLoadingAutomationPeer(this);
        }

        private static bool IsValidIndicatorSize(object value)
        {
            var size = (double)value;
            return !double.IsNaN(size) &&
                !double.IsInfinity(size) &&
                size > 0d;
        }

        private static bool IsValidOrientation(object value)
        {
            var orientation = (Orientation)value;
            return orientation == Orientation.Horizontal ||
                orientation == Orientation.Vertical;
        }

        private static void OnIsLoadingChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var loading = (ZenLoading)dependencyObject;
#if ZENUI_LIVE_REGIONS
            var peer = UIElementAutomationPeer.FromElement(loading);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
#endif
        }

        private sealed class ZenLoadingAutomationPeer : FrameworkElementAutomationPeer
        {
            public ZenLoadingAutomationPeer(ZenLoading owner)
                : base(owner)
            {
            }

            protected override string GetClassNameCore()
            {
                return nameof(ZenLoading);
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.ProgressBar;
            }

            protected override string GetNameCore()
            {
                var name = base.GetNameCore();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }

                var loading = (ZenLoading)Owner;
                return string.IsNullOrWhiteSpace(loading.LoadingText)
                    ? "正在加载"
                    : loading.LoadingText;
            }
        }
    }
}
