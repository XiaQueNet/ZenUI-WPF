using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持语义外观与自定义圆角的按钮。
    /// </summary>
    public class ZenButton : Button
    {
        private static readonly Type SelfType = typeof(ZenButton);

        static ZenButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置按钮圆角。
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
                    default(CornerRadius),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置按钮的语义外观。
        /// </summary>
        [Bindable(true)]
        public ButtonVariant Variant
        {
            get { return (ButtonVariant)GetValue(VariantProperty); }
            set { SetValue(VariantProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Variant"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.Register(
                nameof(Variant),
                typeof(ButtonVariant),
                SelfType,
                new FrameworkPropertyMetadata(ButtonVariant.Primary));

        /// <summary>
        /// 获取或设置按钮的视觉呈现形式。
        /// </summary>
        [Bindable(true)]
        public ButtonAppearance Appearance
        {
            get { return (ButtonAppearance)GetValue(AppearanceProperty); }
            set { SetValue(AppearanceProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Appearance"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty AppearanceProperty =
            DependencyProperty.Register(
                nameof(Appearance),
                typeof(ButtonAppearance),
                SelfType,
                new FrameworkPropertyMetadata(ButtonAppearance.Filled));

        /// <summary>
        /// 获取或设置鼠标悬停时的背景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HoverBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register(
                nameof(HoverBackground),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置按下时的背景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="PressedBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register(
                nameof(PressedBackground),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置鼠标悬停时的前景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush HoverForeground
        {
            get { return (Brush)GetValue(HoverForegroundProperty); }
            set { SetValue(HoverForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HoverForeground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register(
                nameof(HoverForeground),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置按下时的前景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush PressedForeground
        {
            get { return (Brush)GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="PressedForeground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register(
                nameof(PressedForeground),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置鼠标悬停时的边框画刷。
        /// </summary>
        [Bindable(true)]
        public Brush HoverBorderBrush
        {
            get { return (Brush)GetValue(HoverBorderBrushProperty); }
            set { SetValue(HoverBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HoverBorderBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HoverBorderBrushProperty =
            DependencyProperty.Register(
                nameof(HoverBorderBrush),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置按下时的边框画刷。
        /// </summary>
        [Bindable(true)]
        public Brush PressedBorderBrush
        {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="PressedBorderBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register(
                nameof(PressedBorderBrush),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认背景画刷。
        /// </summary>
        internal Brush ThemeBackground
        {
            get { return (Brush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBackground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeBackgroundProperty =
            RegisterThemeBrush(nameof(ThemeBackground));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认前景画刷。
        /// </summary>
        internal Brush ThemeForeground
        {
            get { return (Brush)GetValue(ThemeForegroundProperty); }
            set { SetValue(ThemeForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeForeground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeForegroundProperty =
            RegisterThemeBrush(nameof(ThemeForeground));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认边框画刷。
        /// </summary>
        internal Brush ThemeBorderBrush
        {
            get { return (Brush)GetValue(ThemeBorderBrushProperty); }
            set { SetValue(ThemeBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBorderBrush"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeBorderBrushProperty =
            RegisterThemeBrush(nameof(ThemeBorderBrush));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认悬浮背景画刷。
        /// </summary>
        internal Brush ThemeHoverBackground
        {
            get { return (Brush)GetValue(ThemeHoverBackgroundProperty); }
            set { SetValue(ThemeHoverBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeHoverBackground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeHoverBackgroundProperty =
            RegisterThemeBrush(nameof(ThemeHoverBackground));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认悬浮前景画刷。
        /// </summary>
        internal Brush ThemeHoverForeground
        {
            get { return (Brush)GetValue(ThemeHoverForegroundProperty); }
            set { SetValue(ThemeHoverForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeHoverForeground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeHoverForegroundProperty =
            RegisterThemeBrush(nameof(ThemeHoverForeground));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认悬浮边框画刷。
        /// </summary>
        internal Brush ThemeHoverBorderBrush
        {
            get { return (Brush)GetValue(ThemeHoverBorderBrushProperty); }
            set { SetValue(ThemeHoverBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeHoverBorderBrush"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeHoverBorderBrushProperty =
            RegisterThemeBrush(nameof(ThemeHoverBorderBrush));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认按下背景画刷。
        /// </summary>
        internal Brush ThemePressedBackground
        {
            get { return (Brush)GetValue(ThemePressedBackgroundProperty); }
            set { SetValue(ThemePressedBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemePressedBackground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemePressedBackgroundProperty =
            RegisterThemeBrush(nameof(ThemePressedBackground));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认按下前景画刷。
        /// </summary>
        internal Brush ThemePressedForeground
        {
            get { return (Brush)GetValue(ThemePressedForegroundProperty); }
            set { SetValue(ThemePressedForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemePressedForeground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemePressedForegroundProperty =
            RegisterThemeBrush(nameof(ThemePressedForeground));

        /// <summary>
        /// 获取或设置由按钮语义外观提供的默认按下边框画刷。
        /// </summary>
        internal Brush ThemePressedBorderBrush
        {
            get { return (Brush)GetValue(ThemePressedBorderBrushProperty); }
            set { SetValue(ThemePressedBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemePressedBorderBrush"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemePressedBorderBrushProperty =
            RegisterThemeBrush(nameof(ThemePressedBorderBrush));

        private static DependencyProperty RegisterThemeBrush(string name)
        {
            return DependencyProperty.Register(
                name,
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        }
    }
}
