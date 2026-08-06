using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

using ZenUI.Wpf.Converters;

namespace ZenUI.Wpf.ModernCompatibilityTests
{
    internal static class ConverterContracts
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public static void Verify()
        {
            VerifyBoolConverter();
            VerifyNullConverter();
            VerifyEnumerableConverter();
            VerifyComparisonConverter();
        }

        private static void VerifyBoolConverter()
        {
            var converter = new BoolToVisibilityConverter();
            AssertVisibility(Visibility.Visible, converter.Convert(true, typeof(Visibility), null, Culture));
            AssertVisibility(Visibility.Collapsed, converter.Convert(false, typeof(Visibility), null, Culture));
            ContractAssert.AreEqual(
                true,
                (bool)converter.ConvertBack(Visibility.Visible, typeof(bool), null, Culture),
                "BoolToVisibilityConverter 反向转换 Visible 失败。");

            converter.IsInverted = true;
            AssertVisibility(Visibility.Collapsed, converter.Convert(true, typeof(Visibility), null, Culture));
            converter.UseCollapsed = false;
            AssertVisibility(Visibility.Hidden, converter.Convert(true, typeof(Visibility), null, Culture));
        }

        private static void VerifyNullConverter()
        {
            var converter = new NullToVisibilityConverter();
            AssertVisibility(Visibility.Collapsed, converter.Convert(null, typeof(Visibility), null, Culture));
            AssertVisibility(Visibility.Collapsed, converter.Convert(string.Empty, typeof(Visibility), null, Culture));
            AssertVisibility(Visibility.Visible, converter.Convert("value", typeof(Visibility), null, Culture));
        }

        private static void VerifyEnumerableConverter()
        {
            var converter = new EnumerableToVisibilityConverter();
            AssertVisibility(
                Visibility.Collapsed,
                converter.Convert(Array.Empty<object>(), typeof(Visibility), null, Culture));
            AssertVisibility(
                Visibility.Visible,
                converter.Convert(new List<int> { 1 }, typeof(Visibility), null, Culture));
        }

        private static void VerifyComparisonConverter()
        {
            var cases = new[]
            {
                new ComparisonCase(ComparisonType.Equal, 5, "5", true),
                new ComparisonCase(ComparisonType.NotEqual, 5, "4", true),
                new ComparisonCase(ComparisonType.GreaterThan, 5, "4", true),
                new ComparisonCase(ComparisonType.GreaterThanOrEqual, 5, "5", true),
                new ComparisonCase(ComparisonType.LessThan, 4, "5", true),
                new ComparisonCase(ComparisonType.LessThanOrEqual, 5, "5", true),
                new ComparisonCase(ComparisonType.Equal, 5, "4", false)
            };
            foreach (var item in cases)
            {
                var converter = new ComparisonToVisibilityConverter
                {
                    Comparison = item.Comparison
                };
                AssertVisibility(
                    item.ExpectedVisible ? Visibility.Visible : Visibility.Collapsed,
                    converter.Convert(item.Value, typeof(Visibility), item.Parameter, Culture));
            }

            var invalidConverter = new ComparisonToVisibilityConverter();
            AssertVisibility(
                Visibility.Collapsed,
                invalidConverter.Convert(
                    DependencyProperty.UnsetValue,
                    typeof(Visibility),
                    "5",
                    Culture));
            AssertVisibility(
                Visibility.Collapsed,
                invalidConverter.Convert(5, typeof(Visibility), "not-a-number", Culture));
        }

        private static void AssertVisibility(Visibility expected, object actual)
        {
            ContractAssert.AreEqual(
                expected,
                (Visibility)actual,
                "转换器可见性结果不正确。");
        }

        private sealed class ComparisonCase
        {
            public ComparisonCase(
                ComparisonType comparison,
                int value,
                string parameter,
                bool expectedVisible)
            {
                Comparison = comparison;
                Value = value;
                Parameter = parameter;
                ExpectedVisible = expectedVisible;
            }

            public ComparisonType Comparison { get; }

            public int Value { get; }

            public string Parameter { get; }

            public bool ExpectedVisible { get; }
        }
    }
}
