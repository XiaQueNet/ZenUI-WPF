using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观的滑块。
    /// </summary>
    public class ZenSlider : Slider
    {
        static ZenSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenSlider),
                new FrameworkPropertyMetadata(typeof(ZenSlider)));
        }

        /// <summary>
        /// 获取或设置滑块轨道的厚度。
        /// </summary>
        [Bindable(true)]
        public double TrackThickness
        {
            get { return (double)GetValue(TrackThicknessProperty); }
            set { SetValue(TrackThicknessProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TrackThickness"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TrackThicknessProperty =
            DependencyProperty.Register(
                nameof(TrackThickness),
                typeof(double),
                typeof(ZenSlider),
                new FrameworkPropertyMetadata(4d));

        /// <summary>
        /// 获取或设置滑块手柄的填充画刷。
        /// </summary>
        [Bindable(true)]
        public Brush ThumbBrush
        {
            get { return (Brush)GetValue(ThumbBrushProperty); }
            set { SetValue(ThumbBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThumbBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ThumbBrushProperty =
            DependencyProperty.Register(
                nameof(ThumbBrush),
                typeof(Brush),
                typeof(ZenSlider),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置鼠标悬停时滑块手柄的填充画刷。
        /// </summary>
        [Bindable(true)]
        public Brush HoverThumbBrush
        {
            get { return (Brush)GetValue(HoverThumbBrushProperty); }
            set { SetValue(HoverThumbBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HoverThumbBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HoverThumbBrushProperty =
            DependencyProperty.Register(
                nameof(HoverThumbBrush),
                typeof(Brush),
                typeof(ZenSlider),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
