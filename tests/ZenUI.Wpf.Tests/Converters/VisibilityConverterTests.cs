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
        public void BoolConverterSupportsForwardReverseAndConvertBack()
        {
            var converter = new BoolToVisibilityConverter();

            Assert.AreEqual(
                Visibility.Visible,
                converter.Convert(true, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(false, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                true,
                converter.ConvertBack(Visibility.Visible, typeof(bool), null, Culture));

            converter.IsReverse = true;
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(true, typeof(Visibility), null, Culture));
        }

        [TestMethod]
        public void NullAndEnumerableConvertersHandleEmptyValues()
        {
            var nullConverter = new NullToVisibilityConverter();
            var enumerableConverter = new EnumerableToVisibilityConverter();

            Assert.AreEqual(
                Visibility.Collapsed,
                nullConverter.Convert(string.Empty, typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Visible,
                nullConverter.Convert("value", typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                enumerableConverter.Convert(Array.Empty<object>(), typeof(Visibility), null, Culture));
            Assert.AreEqual(
                Visibility.Visible,
                enumerableConverter.Convert(new List<int> { 1 }, typeof(Visibility), null, Culture));
        }

        [TestMethod]
        public void ComparisonConverterSupportsNumericComparison()
        {
            var converter = new ComparisonToVisibilityConverter
            {
                Comparison = ComparisonType.GreaterThan
            };

            Assert.AreEqual(
                Visibility.Visible,
                converter.Convert(10, typeof(Visibility), "5", Culture));
            Assert.AreEqual(
                Visibility.Collapsed,
                converter.Convert(3, typeof(Visibility), "5", Culture));
        }
    }
}
