using System;
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
