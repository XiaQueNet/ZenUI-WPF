using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Theming;

namespace ZenUI.Wpf.Tests.Theming
{
    [STATestClass]
    public class ZenThemeManagerTests
    {
        [TestMethod]
        public void ThemesCanSwitch()
        {
            var window = CreateTestWindow();

            try
            {
                window.Show();
                var resources = AddGenericTheme(window);

                ZenThemeManager.ApplyTheme(resources, ZenTheme.Dark, false);
                Assert.AreEqual(
                    Color.FromRgb(0x1D, 0x21, 0x29),
                    ((SolidColorBrush)resources["ZenSurfaceBrush"]).Color);

                ZenThemeManager.ApplyTheme(resources, ZenTheme.HighContrast, false);
                Assert.IsNotNull(resources["ZenFocusBrush"]);
                Assert.AreEqual(
                    SystemColors.HighlightTextColor,
                    ((SolidColorBrush)resources["ZenOnAccentBrush"]).Color);
                Assert.AreEqual(
                    SystemColors.WindowColor,
                    ((SolidColorBrush)resources["ZenControlThumbBrush"]).Color);
                Assert.AreEqual(
                    SystemColors.WindowTextColor,
                    ((SolidColorBrush)resources["ZenControlThumbBorderBrush"]).Color);
                Assert.AreEqual(2, resources.MergedDictionaries.Count);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void LightThemeRemovesThemeOverrides()
        {
            var window = CreateTestWindow();

            try
            {
                window.Show();
                var resources = AddGenericTheme(window);
                ZenThemeManager.ApplyTheme(resources, ZenTheme.Dark, false);

                ZenThemeManager.ApplyTheme(resources, ZenTheme.Light, false);

                Assert.AreEqual(1, resources.MergedDictionaries.Count);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void NullResourcesAreRejected()
        {
            Assert.ThrowsExactly<ArgumentNullException>(
                () => ZenThemeManager.ApplyTheme(null, ZenTheme.Light, false));
        }

        [TestMethod]
        public void ColorThemesExposeTheSameTokenContract()
        {
            var lightTokens = LoadTokenContract("Colors.xaml");
            var darkTokens = LoadTokenContract("Dark.xaml");
            var highContrastTokens = LoadTokenContract("HighContrast.xaml");

            AssertTokenContract(lightTokens, darkTokens, "Dark");
            AssertTokenContract(lightTokens, highContrastTokens, "HighContrast");
        }

        private static Dictionary<object, Type> LoadTokenContract(string fileName)
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/" + fileName,
                    UriKind.Relative)
            };
            var resources = new Dictionary<object, Type>();
            AddResources(dictionary, resources);
            return resources;
        }

        private static void AddResources(
            ResourceDictionary dictionary,
            IDictionary<object, Type> resources)
        {
            foreach (var mergedDictionary in dictionary.MergedDictionaries)
            {
                AddResources(mergedDictionary, resources);
            }

            foreach (var key in dictionary.Keys.Cast<object>())
            {
                var value = dictionary[key];
                Assert.IsNotNull(value, $"Theme token '{key}' resolved to null.");
                resources[key] = value.GetType();
            }
        }

        private static void AssertTokenContract(
            Dictionary<object, Type> expected,
            Dictionary<object, Type> actual,
            string themeName)
        {
            CollectionAssert.AreEquivalent(
                expected.Keys.Select(key => key.ToString()).ToArray(),
                actual.Keys.Select(key => key.ToString()).ToArray(),
                $"{themeName} must define exactly the same color token keys as Light.");

            foreach (var token in expected)
            {
                Assert.AreEqual(
                    token.Value,
                    actual[token.Key],
                    $"{themeName} token '{token.Key}' must keep the Light token type.");
            }
        }

        private static ResourceDictionary AddGenericTheme(Window window)
        {
            var resources = window.Resources;
            resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            });
            return resources;
        }

        private static Window CreateTestWindow()
        {
            return new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 100,
                Height = 100
            };
        }
    }
}
