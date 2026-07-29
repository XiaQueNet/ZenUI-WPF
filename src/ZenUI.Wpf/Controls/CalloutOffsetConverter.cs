using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 计算气泡相对于触发器的居中偏移。
    /// </summary>
    public sealed class CalloutOffsetConverter : Freezable, IMultiValueConverter
    {
        public double Gap
        {
            get { return (double)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register(
                nameof(Gap),
                typeof(double),
                typeof(CalloutOffsetConverter),
                new FrameworkPropertyMetadata(0d));

        /// <inheritdoc/>
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values == null || values.Length < 3)
            {
                return 0d;
            }

            var targetSize = ToFiniteDouble(values[0]);
            var calloutSize = ToFiniteDouble(values[1]);
            var placement = values[2] is PlacementMode mode
                ? mode
                : PlacementMode.Top;
            var requestedOffset = values.Length > 3
                ? ToFiniteDouble(values[3])
                : 0d;
            var gap = values.Length > 4
                ? ToFiniteDouble(values[4])
                : Gap;
            var axis = parameter as string;

            if (string.Equals(axis, "Horizontal", StringComparison.Ordinal))
            {
                if (placement == PlacementMode.Top || placement == PlacementMode.Bottom)
                {
                    return requestedOffset + ((targetSize - calloutSize) / 2d);
                }

                if (placement == PlacementMode.Left)
                {
                    return requestedOffset - gap;
                }

                if (placement == PlacementMode.Right)
                {
                    return requestedOffset + gap;
                }
            }

            if (string.Equals(axis, "Vertical", StringComparison.Ordinal))
            {
                if (placement == PlacementMode.Left || placement == PlacementMode.Right)
                {
                    return requestedOffset + ((targetSize - calloutSize) / 2d);
                }

                if (placement == PlacementMode.Top)
                {
                    return requestedOffset - gap;
                }

                if (placement == PlacementMode.Bottom)
                {
                    return requestedOffset + gap;
                }
            }

            return requestedOffset;
        }

        /// <inheritdoc/>
        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new CalloutOffsetConverter();
        }

        private static double ToFiniteDouble(object value)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue ||
                value == Binding.DoNothing)
            {
                return 0d;
            }

            try
            {
                var converted = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return double.IsNaN(converted) || double.IsInfinity(converted)
                    ? 0d
                    : converted;
            }
            catch (Exception)
            {
                return 0d;
            }
        }
    }
}
