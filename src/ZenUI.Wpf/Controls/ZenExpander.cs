using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观、可展开或折叠内容的控件。
    /// </summary>
    public class ZenExpander : Expander
    {
        private static readonly Type SelfType = typeof(ZenExpander);

        static ZenExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置控件外框的圆角半径。
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
                new FrameworkPropertyMetadata(
                    new CornerRadius(8),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置标题区域的内边距。
        /// </summary>
        [Bindable(true)]
        public Thickness HeaderPadding
        {
            get { return (Thickness)GetValue(HeaderPaddingProperty); }
            set { SetValue(HeaderPaddingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HeaderPadding"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register(
                nameof(HeaderPadding),
                typeof(Thickness),
                SelfType,
                new FrameworkPropertyMetadata(
                    new Thickness(14, 12, 14, 12),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 获取或设置标题中展开标识的边长。
        /// </summary>
        [Bindable(true)]
        public double GlyphSize
        {
            get { return (double)GetValue(GlyphSizeProperty); }
            set { SetValue(GlyphSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="GlyphSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty GlyphSizeProperty =
            DependencyProperty.Register(
                nameof(GlyphSize),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(
                    16d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidGlyphSize);

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenExpanderAutomationPeer(this);
        }

        private static bool IsValidGlyphSize(object value)
        {
            var size = (double)value;
            return !double.IsNaN(size) &&
                !double.IsInfinity(size) &&
                size >= 0d;
        }

        private sealed class ZenExpanderAutomationPeer : ExpanderAutomationPeer
        {
            public ZenExpanderAutomationPeer(ZenExpander owner)
                : base(owner)
            {
            }

            protected override string GetClassNameCore()
            {
                return nameof(ZenExpander);
            }
        }
    }
}
