using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class CheckBoxTests
    {
        [TestMethod]
        public void CheckedIndicatorUsesIndependentCustomBrushes()
        {
            var checkedBackground = new SolidColorBrush(Colors.Purple);
            var checkedBorderBrush = new SolidColorBrush(Colors.Indigo);
            var hoverBorderBrush = new SolidColorBrush(Colors.Orange);
            var checkMarkForeground = new SolidColorBrush(Colors.Gold);
            var checkBox = new ZenCheckBox
            {
                CheckedBackground = checkedBackground,
                CheckedBorderBrush = checkedBorderBrush,
                CheckedGlyphBrush = checkMarkForeground,
                Content = "复选",
                HoverBorderBrush = hoverBorderBrush,
                IsChecked = true
            };
            var window = CreateTestWindow(checkBox, 180, 80);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                window.UpdateLayout();

                var box = (Border)checkBox.Template.FindName("Box", checkBox);
                var checkMark = (Path)checkBox.Template.FindName("CheckMark", checkBox);
                Assert.AreSame(checkedBackground, box.Background);
                Assert.AreSame(checkedBorderBrush, box.BorderBrush);
                Assert.AreSame(checkMarkForeground, checkMark.Stroke);
                Assert.AreSame(hoverBorderBrush, checkBox.HoverBorderBrush);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void IndeterminateIndicatorUsesCheckedStateBrushes()
        {
            var checkedBackground = new SolidColorBrush(Colors.Teal);
            var checkedBorderBrush = new SolidColorBrush(Colors.DarkCyan);
            var checkBox = new ZenCheckBox
            {
                CheckedBackground = checkedBackground,
                CheckedBorderBrush = checkedBorderBrush,
                Content = "三态复选",
                IsChecked = null,
                IsThreeState = true
            };
            var window = CreateTestWindow(checkBox, 180, 80);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                window.UpdateLayout();

                var box = (Border)checkBox.Template.FindName("Box", checkBox);
                Assert.AreSame(checkedBackground, box.Background);
                Assert.AreSame(checkedBorderBrush, box.BorderBrush);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
