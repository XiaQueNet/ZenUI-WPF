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
            Assert.AreEqual(new Thickness(10, 6, 10, 6), resources["ZenListBoxItemPadding"]);
            Assert.AreEqual(2, resources.MergedDictionaries.Count);

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Comfortable);

            Assert.AreEqual(40d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(new Thickness(14, 11, 14, 11), resources["ZenListBoxItemPadding"]);
            Assert.AreEqual(2, resources.MergedDictionaries.Count);

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Standard);

            Assert.AreEqual(36d, resources["ZenInputControlMinHeight"]);
            Assert.AreEqual(new Thickness(12, 9, 12, 9), resources["ZenListBoxItemPadding"]);
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
