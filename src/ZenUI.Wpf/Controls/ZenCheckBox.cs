using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观的复选框。
    /// </summary>
    public class ZenCheckBox : CheckBox
    {
        private static readonly System.Type SelfType = typeof(ZenCheckBox);

        static ZenCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置选中或不确定状态下选择标识的背景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush CheckedBackground
        {
            get { return (Brush)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CheckedBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CheckedBackgroundProperty =
            RegisterBrush(nameof(CheckedBackground));

        /// <summary>
        /// 获取或设置选中或不确定状态下选择标识的边框画刷。
        /// </summary>
        [Bindable(true)]
        public Brush CheckedBorderBrush
        {
            get { return (Brush)GetValue(CheckedBorderBrushProperty); }
            set { SetValue(CheckedBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CheckedBorderBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CheckedBorderBrushProperty =
            RegisterBrush(nameof(CheckedBorderBrush));

        /// <summary>
        /// 获取或设置鼠标悬停时选择标识的边框画刷。
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
            RegisterBrush(nameof(HoverBorderBrush));

        /// <summary>
        /// 获取或设置勾号或不确定标记的前景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush CheckedGlyphBrush
        {
            get { return (Brush)GetValue(CheckedGlyphBrushProperty); }
            set { SetValue(CheckedGlyphBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CheckedGlyphBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CheckedGlyphBrushProperty =
            RegisterBrush(nameof(CheckedGlyphBrush));

        /// <summary>
        /// 获取或设置左侧选择标识的边长。该值必须为大于或等于零的有限值。
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
                    18d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidIndicatorSize);

        /// <summary>
        /// 获取或设置由复选状态提供的默认背景画刷。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBackground
        {
            get { return (Brush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ThemeBackgroundProperty =
            RegisterBrush(nameof(ThemeBackground));

        /// <summary>
        /// 获取或设置由复选状态提供的默认边框画刷。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBorderBrush
        {
            get { return (Brush)GetValue(ThemeBorderBrushProperty); }
            set { SetValue(ThemeBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBorderBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ThemeBorderBrushProperty =
            RegisterBrush(nameof(ThemeBorderBrush));

        private static DependencyProperty RegisterBrush(string name)
        {
            return DependencyProperty.Register(
                name,
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));
        }

        private static bool IsValidIndicatorSize(object value)
        {
            var size = (double)value;
            return !double.IsNaN(size) &&
                !double.IsInfinity(size) &&
                size >= 0d;
        }
    }
}
