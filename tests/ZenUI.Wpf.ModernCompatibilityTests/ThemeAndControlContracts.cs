using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

namespace ZenUI.Wpf.ModernCompatibilityTests
{
    internal static class ThemeAndControlContracts
    {
        public static void VerifyThemesAndDensities()
        {
            EnsureApplication();
            var resources = LoadGenericTheme();

            ZenThemeManager.ApplyTheme(resources, ZenTheme.Dark, false);
            ContractAssert.AreEqual(
                Color.FromRgb(0x1D, 0x21, 0x29),
                ((SolidColorBrush)resources["ZenSurfaceBrush"]).Color,
                "Dark 主题表面色不正确。");

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Compact);
            ContractAssert.AreEqual(
                32d,
                (double)resources["ZenInputControlMinHeight"],
                "Compact 输入控件高度不正确。");
            ContractAssert.AreEqual(
                36d,
                (double)resources["ZenDateTimePickerCalendarCellWidth"],
                "Compact 日期时间日历单元宽度不正确。");

            ZenThemeManager.ApplyTheme(resources, ZenTheme.HighContrast, false);
            ContractAssert.IsNotNull(
                resources["ZenFocusBrush"],
                "HighContrast 主题缺少焦点画刷。");
            ContractAssert.AreEqual(
                SystemColors.WindowColor,
                ((SolidColorBrush)resources["ZenSurfaceBrush"]).Color,
                "HighContrast 主题表面色不正确。");

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Comfortable);
            ContractAssert.AreEqual(
                40d,
                (double)resources["ZenInputControlMinHeight"],
                "Comfortable 输入控件高度不正确。");
            ContractAssert.AreEqual(
                44d,
                (double)resources["ZenDateTimePickerCalendarCellWidth"],
                "Comfortable 日期时间日历单元宽度不正确。");

            ZenDensityManager.ApplyDensity(resources, ZenDensity.Standard);
            ZenThemeManager.ApplyTheme(resources, ZenTheme.Light, false);
            ContractAssert.AreEqual(
                36d,
                (double)resources["ZenInputControlMinHeight"],
                "Standard 输入控件高度不正确。");
        }

        public static void VerifyControlStylesAndTemplates()
        {
            var application = EnsureApplication();
            application.Resources.MergedDictionaries.Clear();
            application.Resources.MergedDictionaries.Add(LoadGenericTheme());

            var controlTypes = new[]
            {
                typeof(ZenAlert),
                typeof(ZenButton),
                typeof(ZenCalendar),
                typeof(ZenCheckBox),
                typeof(ZenComboBox),
                typeof(ZenContextMenu),
                typeof(ZenDataGrid),
                typeof(ZenDatePicker),
                typeof(ZenDateTimePicker),
                typeof(ZenExpander),
                typeof(ZenListBox),
                typeof(ZenLoading),
                typeof(ZenMenuItem),
                typeof(ZenNumberBox),
                typeof(ZenPasswordBox),
                typeof(ZenPopover),
                typeof(ZenProgressBar),
                typeof(ZenRadioButton),
                typeof(ZenRadioGroup),
                typeof(ZenSlider),
                typeof(ZenSwitch),
                typeof(ZenTextBox),
                typeof(ZenTimePicker)
            };
            foreach (var controlType in controlTypes)
            {
                ContractAssert.IsTrue(
                    application.TryFindResource(controlType) is Style,
                    controlType.FullName + " 默认样式未能加载。");
            }

            var controls = new Control[]
            {
                new ZenAlert { Content = "兼容性测试" },
                new ZenButton { Content = "按钮" },
                new ZenCheckBox { Content = "复选" },
                new ZenComboBox { ItemsSource = new[] { "第一项", "第二项" }, SelectedIndex = 0 },
                new ZenDatePicker { SelectedDate = new DateTime(2026, 8, 6) },
                new ZenDateTimePicker { SelectedDateTime = new DateTime(2026, 8, 6, 14, 30, 0) },
                new ZenNumberBox { Value = 3, Increment = 2 },
                new ZenPasswordBox { Watermark = "密码" },
                new ZenProgressBar { Maximum = 10, Value = 6 },
                new ZenRadioButton { Content = "单选" },
                new ZenSlider { Minimum = 0, Maximum = 10, Value = 5 },
                new ZenSwitch { CheckedContent = "开", UncheckedContent = "关", IsChecked = true },
                new ZenTextBox { Text = "文本" },
                new ZenTimePicker { SelectedTime = new TimeSpan(14, 30, 0) }
            };
            var panel = new StackPanel();
            foreach (var control in controls)
            {
                panel.Children.Add(control);
            }

            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 480,
                Height = 800,
                Content = new ScrollViewer { Content = panel }
            };
            try
            {
                window.Show();
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                foreach (var control in controls)
                {
                    control.ApplyTemplate();
                    ContractAssert.IsNotNull(
                        control.Template,
                        control.GetType().FullName + " 模板未能实例化。");
                    ContractAssert.IsTrue(
                        control.ActualWidth > 0d && control.ActualHeight > 0d,
                        control.GetType().FullName + " 未能完成有效布局。");
                }

                var alert = (ZenAlert)controls[0];
                ContractAssert.AreEqual(
                    AutomationLiveSetting.Polite,
                    AutomationProperties.GetLiveSetting(alert),
                    "ZenAlert Live Region 语义未启用。");
                var numberBox = (ZenNumberBox)controls[6];
                ContractAssert.AreEqual(2m, numberBox.Increment, "ZenNumberBox.Increment 未保留设置值。");
            }
            finally
            {
                window.Close();
            }
        }

        private static ResourceDictionary LoadGenericTheme()
        {
            return new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
        }

        private static Application EnsureApplication()
        {
            return Application.Current ?? new Application();
        }
    }
}
