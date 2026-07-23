using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Converters;

namespace ZenUI.Wpf.Tests.Converters
{
    [TestClass]
    public class VisibilityConverterTests
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        [TestMethod]
        public void BoolConverterConvertsBooleanValues()
        {
            var converter = new BoolToVisibilityConverter();

            Assert.AreEqual(
                Visibility.Visible,
                converter.Convert(true, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(false, typeof(Visibility), null, Culture));
        }

        [TestMethod]
        public void BoolConverterAppliesReverseOption()
        {
            var converter = new BoolToVisibilityConverter
            {
                IsReverse = true
            };

            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(true, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Visible,
                converter.Convert(false, typeof(Visibility), null, Culture));
        }

        [TestMethod]
        public void BoolConverterConvertsVisibilityBack()
        {
            var converter = new BoolToVisibilityConverter();

            Assert.AreEqual(
                true,
                converter.ConvertBack(Visibility.Visible, typeof(bool), null, Culture));
            Assert.AreEqual(
                false,
                converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, Culture));

            converter.IsReverse = true;
            Assert.AreEqual(
                false,
                converter.ConvertBack(Visibility.Visible, typeof(bool), null, Culture));
        }

        [TestMethod]
        public void VisibilityConvertersUseHiddenWhenConfigured()
        {
            var converter = new BoolToVisibilityConverter
            {
                UseCollapsed = false
            };

            Assert.AreEqual(
                Visibility.Hidden,
                converter.Convert(false, typeof(Visibility), null, Culture));
        }

        [TestMethod]
        public void NullConverterHandlesNullEmptyAndPopulatedValues()
        {
            var converter = new NullToVisibilityConverter();

            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(null, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(string.Empty, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Visible,
                converter.Convert("value", typeof(Visibility), null, Culture));
        }

        [TestMethod]
        public void EnumerableConverterHandlesEmptyAndPopulatedCollections()
        {
            var converter = new EnumerableToVisibilityConverter();

            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(Array.Empty<object>(), typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Visible,
                converter.Convert(new List<int> { 1 }, typeof(Visibility), null, Culture));
        }

        [TestMethod]
        [DataRow(ComparisonType.Equal, 5, "5", true)]
        [DataRow(ComparisonType.Equal, 5, "4", false)]
        [DataRow(ComparisonType.NotEqual, 5, "4", true)]
        [DataRow(ComparisonType.NotEqual, 5, "5", false)]
        [DataRow(ComparisonType.GreaterThan, 5, "4", true)]
        [DataRow(ComparisonType.GreaterThan, 5, "5", false)]
        [DataRow(ComparisonType.GreaterThanOrEqual, 5, "5", true)]
        [DataRow(ComparisonType.GreaterThanOrEqual, 4, "5", false)]
        [DataRow(ComparisonType.LessThan, 4, "5", true)]
        [DataRow(ComparisonType.LessThan, 5, "5", false)]
        [DataRow(ComparisonType.LessThanOrEqual, 5, "5", true)]
        [DataRow(ComparisonType.LessThanOrEqual, 6, "5", false)]
        public void ComparisonConverterSupportsEachComparisonMode(
            ComparisonType comparison,
            int value,
            string parameter,
            bool expectedVisible)
        {
            var converter = new ComparisonToVisibilityConverter
            {
                Comparison = comparison
            };

            var result = converter.Convert(value, typeof(Visibility), parameter, Culture);

            Assert.AreEqual(
                expectedVisible ? Visibility.Visible : Visibility.Collapsed,
                result);
        }

        [TestMethod]
        public void ComparisonConverterReturnsInvisibleForInvalidInput()
        {
            var converter = new ComparisonToVisibilityConverter();

            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(DependencyProperty.UnsetValue, typeof(Visibility), "5", Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(5, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(5, typeof(Visibility), "not-a-number", Culture));
        }
    }
}
