using System;
using System.Linq;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

namespace ZenUI.Wpf.Tests.Theming
{
    [STATestClass]
    public class ZenDensityManagerTests
    {
        [TestMethod]
        public void DensitiesCanSwitchIndependently()
        {
            var resources = LoadGenericTheme();

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Compact);

            Assert.AreEqual(32d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(32d, resources["ZenButtonMinHeight"]);
            Assert.AreEqual(new Thickness(10, 6, 10, 6), resources["ZenListBoxItemPadding"]);
            Assert.AreEqual(32d, resources["ZenNumberBoxSpinButtonWidth"]);
            Assert.AreEqual(36d, resources["ZenDataGridRowMinHeight"]);
            Assert.AreEqual(328d, resources["ZenCalendarPopupWidth"]);
            Assert.AreEqual(new Thickness(6), resources["ZenCalendarContentMargin"]);
            Assert.AreEqual(172d, resources["ZenTimePickerListHeight"]);
            Assert.AreEqual(32d, resources["ZenTimePickerItemHeight"]);
            Assert.AreEqual(52d, resources["ZenSwitchWidth"]);
            Assert.AreEqual(16d, resources["ZenSelectionIndicatorSize"]);
            Assert.AreEqual(16d, resources["ZenSliderThumbSize"]);
            Assert.AreEqual(3d, resources["ZenSliderTrackThickness"]);
            Assert.AreEqual(6d, resources["ZenProgressBarThickness"]);
            Assert.AreEqual(new Thickness(12, 8, 12, 8), resources["ZenAlertPadding"]);
            Assert.AreEqual(2, resources.MergedDictionaries.Count);

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Comfortable);

            Assert.AreEqual(40d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(40d, resources["ZenButtonMinHeight"]);
            Assert.AreEqual(new Thickness(14, 11, 14, 11), resources["ZenListBoxItemPadding"]);
            Assert.AreEqual(40d, resources["ZenNumberBoxSpinButtonWidth"]);
            Assert.AreEqual(52d, resources["ZenDataGridRowMinHeight"]);
            Assert.AreEqual(412d, resources["ZenCalendarPopupWidth"]);
            Assert.AreEqual(new Thickness(10), resources["ZenCalendarContentMargin"]);
            Assert.AreEqual(220d, resources["ZenTimePickerListHeight"]);
            Assert.AreEqual(40d, resources["ZenTimePickerItemHeight"]);
            Assert.AreEqual(68d, resources["ZenSwitchWidth"]);
            Assert.AreEqual(20d, resources["ZenSelectionIndicatorSize"]);
            Assert.AreEqual(20d, resources["ZenSliderThumbSize"]);
            Assert.AreEqual(4d, resources["ZenSliderTrackThickness"]);
            Assert.AreEqual(10d, resources["ZenProgressBarThickness"]);
            Assert.AreEqual(new Thickness(16, 14, 16, 14), resources["ZenAlertPadding"]);
            Assert.AreEqual(2, resources.MergedDictionaries.Count);

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Standard);

            Assert.AreEqual(36d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(36d, resources["ZenButtonMinHeight"]);
            Assert.AreEqual(new Thickness(12, 9, 12, 9), resources["ZenListBoxItemPadding"]);
            Assert.AreEqual(34d, resources["ZenNumberBoxSpinButtonWidth"]);
            Assert.AreEqual(44d, resources["ZenDataGridRowMinHeight"]);
            Assert.AreEqual(368d, resources["ZenCalendarPopupWidth"]);
            Assert.AreEqual(new Thickness(8), resources["ZenCalendarContentMargin"]);
            Assert.AreEqual(196d, resources["ZenTimePickerListHeight"]);
            Assert.AreEqual(36d, resources["ZenTimePickerItemHeight"]);
            Assert.AreEqual(60d, resources["ZenSwitchWidth"]);
            Assert.AreEqual(18d, resources["ZenSelectionIndicatorSize"]);
            Assert.AreEqual(18d, resources["ZenSliderThumbSize"]);
            Assert.AreEqual(4d, resources["ZenSliderTrackThickness"]);
            Assert.AreEqual(8d, resources["ZenProgressBarThickness"]);
            Assert.AreEqual(new Thickness(14, 11, 14, 11), resources["ZenAlertPadding"]);
            Assert.AreEqual(1, resources.MergedDictionaries.Count);
        }

        [TestMethod]
        public void DensityProfilesExposeTheSameCompatibleContract()
        {
            var standard = LoadGenericTheme();
            var compact = LoadDensityDictionary("Compact.xaml");
            var comfortable = LoadDensityDictionary("Comfortable.xaml");

            CollectionAssert.AreEquivalent(
                compact.Keys.Cast<object>().Select(key => key.ToString()).ToArray(),
                comfortable.Keys.Cast<object>().Select(key => key.ToString()).ToArray());

            foreach (var key in compact.Keys.Cast<object>())
            {
                Assert.IsTrue(standard.Contains(key), $"Standard density token missing: {key}");
                Assert.AreEqual(
                    compact[key].GetType(),
                    comfortable[key].GetType(),
                    $"Comfortable density token type mismatch: {key}");
                Assert.AreEqual(
                    compact[key].GetType(),
                    standard[key].GetType(),
                    $"Standard density token type mismatch: {key}");
            }
        }

        [TestMethod]
        public void DensityAndColorThemeCanSwitchIndependently()
        {
            var resources = LoadGenericTheme();
            ZenThemeManager.ApplyTheme(resources, ZenTheme.Dark, false);
            ZenDensityManager.ApplyDensity(resources, ZenDensity.Compact);

            ZenThemeManager.ApplyTheme(resources, ZenTheme.HighContrast, false);

            Assert.AreEqual(32d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(3, resources.MergedDictionaries.Count);

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Standard);

            Assert.AreEqual(36d, resources["ZenInputControlMinHeight"]);
            Assert.IsNotNull(resources["ZenFocusBrush"]);
            Assert.AreEqual(2, resources.MergedDictionaries.Count);
        }

        [TestMethod]
        public void ApplicationTokenOverrideTakesPriorityOverDensity()
        {
            var resources = LoadGenericTheme();
            resources["ZenInputControlMinHeight"] = 52d;

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Compact);

            Assert.AreEqual(52d, resources["ZenInputControlMinHeight"]);
        }

        [TestMethod]
        public void NullResourcesAreRejected()
        {
            Assert.ThrowsExactly<ArgumentNullException>(
                () => ZenDensityManager.ApplyDensity(null, ZenDensity.Standard));
        }

        [TestMethod]
        public void InvalidDensityIsRejectedWithoutRemovingCurrentDensity()
        {
            var resources = LoadGenericTheme();
            ZenDensityManager.ApplyDensity(resources, ZenDensity.Compact);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => ZenDensityManager.ApplyDensity(resources, (ZenDensity)99));

            Assert.AreEqual(32d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(2, resources.MergedDictionaries.Count);
        }

        private static ResourceDictionary LoadGenericTheme()
        {
            _ = new ZenButton();
            var resources = new ResourceDictionary();
            resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            return resources;
        }

        private static ResourceDictionary LoadDensityDictionary(string fileName)
        {
            return new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Density/" + fileName,
                    UriKind.Relative)
            };
        }
    }
}
