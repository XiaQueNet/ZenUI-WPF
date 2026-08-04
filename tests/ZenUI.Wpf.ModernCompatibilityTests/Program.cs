using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Converters;

namespace ZenUI.Wpf.ModernCompatibilityTests
{
    internal static class Program
    {
        [STAThread]
        private static int Main()
        {
            try
            {
                VerifyControlsAndResources();
                VerifyConverters();
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                return 1;
            }
        }

        private static void VerifyControlsAndResources()
        {
            var application = Application.Current ?? new Application();
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            application.Resources.MergedDictionaries.Add(resources);

            if (!(application.TryFindResource(typeof(ZenButton)) is Style))
            {
                throw new InvalidOperationException("ZenButton 默认样式未能加载。");
            }

            var button = new TestZenButton { Content = "兼容性测试" };
            if (!Equals(button.ExposedDefaultStyleKey, typeof(ZenButton)))
            {
                throw new InvalidOperationException("ZenButton 默认样式键不正确。");
            }

            var alert = new ZenAlert { Content = "加载成功" };
            if (AutomationProperties.GetLiveSetting(alert) != AutomationLiveSetting.Polite)
            {
                throw new InvalidOperationException("ZenAlert Live Region 语义未启用。");
            }
        }

        private static void VerifyConverters()
        {
            var converter = new BoolToVisibilityConverter();
            var result = converter.Convert(
                true,
                typeof(Visibility),
                null,
                CultureInfo.InvariantCulture);

            if (!Equals(result, Visibility.Visible))
            {
                throw new InvalidOperationException("BoolToVisibilityConverter 返回值不正确。");
            }
        }

        private sealed class TestZenButton : ZenButton
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
        }
    }
}
