using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 为 <see cref="ZenRadioGroup"/> 提供带间距和等分能力的布局。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ZenRadioGroupPanel : Panel
    {
        /// <summary>
        /// 获取或设置子元素的排列方向。
        /// </summary>
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
                typeof(ZenRadioGroupPanel),
                new FrameworkPropertyMetadata(
                    Orientation.Horizontal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 获取或设置相邻子元素之间的间距。
        /// </summary>
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Spacing"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(
                nameof(Spacing),
                typeof(double),
                typeof(ZenRadioGroupPanel),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 获取或设置是否在排列方向上为子元素分配相同尺寸。
        /// </summary>
        public bool IsItemWidthUniform
        {
            get { return (bool)GetValue(IsItemWidthUniformProperty); }
            set { SetValue(IsItemWidthUniformProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsItemWidthUniform"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsItemWidthUniformProperty =
            DependencyProperty.Register(
                nameof(IsItemWidthUniform),
                typeof(bool),
                typeof(ZenRadioGroupPanel),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            var visibleCount = GetVisibleChildCount();
            if (visibleCount == 0)
            {
                return new Size();
            }

            var horizontal = Orientation == Orientation.Horizontal;
            var availablePrimary = horizontal ? availableSize.Width : availableSize.Height;
            var availableCross = horizontal ? availableSize.Height : availableSize.Width;
            var totalSpacing = Spacing * Math.Max(0, visibleCount - 1);
            var hasFinitePrimary = !double.IsInfinity(availablePrimary);
            var uniformPrimary = IsItemWidthUniform && hasFinitePrimary
                ? Math.Max(0d, (availablePrimary - totalSpacing) / visibleCount)
                : double.PositiveInfinity;

            var totalPrimary = 0d;
            var maxPrimary = 0d;
            var maxCross = 0d;

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var constraint = horizontal
                    ? new Size(uniformPrimary, availableCross)
                    : new Size(availableCross, uniformPrimary);
                child.Measure(constraint);

                var desiredPrimary = horizontal
                    ? child.DesiredSize.Width
                    : child.DesiredSize.Height;
                var desiredCross = horizontal
                    ? child.DesiredSize.Height
                    : child.DesiredSize.Width;
                totalPrimary += desiredPrimary;
                maxPrimary = Math.Max(maxPrimary, desiredPrimary);
                maxCross = Math.Max(maxCross, desiredCross);
            }

            if (IsItemWidthUniform)
            {
                if (hasFinitePrimary)
                {
                    totalPrimary = availablePrimary - totalSpacing;
                }
                else
                {
                    totalPrimary = maxPrimary * visibleCount;
                }
            }

            totalPrimary += totalSpacing;
            return horizontal
                ? new Size(totalPrimary, maxCross)
                : new Size(maxCross, totalPrimary);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleCount = GetVisibleChildCount();
            if (visibleCount == 0)
            {
                return finalSize;
            }

            var horizontal = Orientation == Orientation.Horizontal;
            var finalPrimary = horizontal ? finalSize.Width : finalSize.Height;
            var finalCross = horizontal ? finalSize.Height : finalSize.Width;
            var totalSpacing = Spacing * Math.Max(0, visibleCount - 1);
            var uniformPrimary = Math.Max(0d, (finalPrimary - totalSpacing) / visibleCount);
            var offset = 0d;

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var desiredPrimary = horizontal
                    ? child.DesiredSize.Width
                    : child.DesiredSize.Height;
                var childPrimary = IsItemWidthUniform ? uniformPrimary : desiredPrimary;
                var rect = horizontal
                    ? new Rect(offset, 0d, childPrimary, finalCross)
                    : new Rect(0d, offset, finalCross, childPrimary);
                child.Arrange(rect);
                offset += childPrimary + Spacing;
            }

            return finalSize;
        }

        private int GetVisibleChildCount()
        {
            var count = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
