using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持自定义圆角的进度条。
    /// </summary>
    public class ZenProgressBar : ProgressBar
    {
        private FrameworkElement track;
        private FrameworkElement indicator;

        static ZenProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenProgressBar),
                new FrameworkPropertyMetadata(typeof(ZenProgressBar)));
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            if (track != null)
            {
                track.SizeChanged -= OnTrackSizeChanged;
            }

            base.OnApplyTemplate();
            track = GetTemplateChild("PART_Track") as FrameworkElement;
            indicator = GetTemplateChild("PART_Indicator") as FrameworkElement;
            if (track != null)
            {
                track.SizeChanged += OnTrackSizeChanged;
            }

            UpdateIndicatorLength();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ValueProperty ||
                e.Property == MinimumProperty ||
                e.Property == MaximumProperty ||
                e.Property == OrientationProperty ||
                e.Property == IsIndeterminateProperty)
            {
                UpdateIndicatorLength();
            }
        }

        /// <summary>
        /// 获取或设置进度条的圆角。
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
                typeof(ZenProgressBar),
                new FrameworkPropertyMetadata(new CornerRadius(4)));

        private void OnTrackSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateIndicatorLength();
        }

        private void UpdateIndicatorLength()
        {
            if (track == null || indicator == null || IsIndeterminate)
            {
                return;
            }

            var range = Maximum - Minimum;
            var ratio = range <= 0d ? 0d : Math.Max(0d, Math.Min(1d, (Value - Minimum) / range));
            if (Orientation == Orientation.Horizontal)
            {
                indicator.Height = double.NaN;
                indicator.Width = track.ActualWidth * ratio;
            }
            else
            {
                indicator.Width = double.NaN;
                indicator.Height = track.ActualHeight * ratio;
            }
        }
    }
}
