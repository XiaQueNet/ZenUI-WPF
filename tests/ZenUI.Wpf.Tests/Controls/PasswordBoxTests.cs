using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class PasswordBoxTests
    {
        [TestMethod]
        public void PasswordBoxTemplateProtectsPasswordAndAppliesWatermarkLayout()
        {
            var textBox = new ZenTextBox { Watermark = "请输入内容" };
            var passwordBox = new ZenPasswordBox { Watermark = "请输入密码" };
            var passwordChangedCount = 0;
            passwordBox.PasswordChanged += (sender, args) => passwordChangedCount++;
            var panel = new StackPanel();
            panel.Children.Add(textBox);
            panel.Children.Add(passwordBox);
            var window = CreateTestWindow(panel, 240, 140);

            try
            {
                window.Show();
                window.UpdateLayout();

                var nativePasswordBox = passwordBox.Template.FindName("PART_PasswordBox", passwordBox) as PasswordBox;
                Assert.IsNotNull(nativePasswordBox);
                nativePasswordBox.Password = "secret";
                Assert.AreEqual(1, passwordChangedCount);
                using (var securePassword = passwordBox.SecurePassword)
                {
                    Assert.AreEqual(6, securePassword.Length);
                }

                var watermark = textBox.Template.FindName("WatermarkText", textBox) as TextBlock;
                var passwordWatermark = passwordBox.Template.FindName("WatermarkText", passwordBox) as TextBlock;
                var passwordWatermarkHost = passwordBox.Template.FindName("WatermarkHost", passwordBox) as Border;
                Assert.IsNotNull(watermark);
                Assert.IsNotNull(passwordWatermark);
                Assert.IsNotNull(passwordWatermarkHost);
                Assert.AreEqual(textBox.Padding, passwordBox.Padding);
                Assert.AreEqual(watermark.HorizontalAlignment, passwordWatermark.HorizontalAlignment);
                Assert.AreEqual(watermark.VerticalAlignment, passwordWatermark.VerticalAlignment);
                Assert.AreEqual(watermark.FontFamily, passwordWatermark.FontFamily);
                Assert.AreEqual(watermark.FontSize, passwordWatermark.FontSize);
                Assert.AreEqual(passwordBox.Padding, passwordWatermarkHost.Margin);
                Assert.AreEqual(new Thickness(), passwordWatermark.Margin);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void PasswordRevealButtonIsVisibleWhenEnabledAndPasswordIsEmpty()
        {
            var passwordBox = new ZenPasswordBox
            {
                IsPasswordRevealButtonEnabled = true,
                Width = 240
            };
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 280,
                Height = 100,
                Content = passwordBox
            };
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

                var revealButton = passwordBox.Template.FindName("PART_RevealButton", passwordBox) as ToggleButton;
                var nativePasswordBox = passwordBox.Template.FindName("PART_PasswordBox", passwordBox) as PasswordBox;
                var revealTextBox = passwordBox.Template.FindName("PART_RevealTextBox", passwordBox) as TextBox;
                Assert.IsNotNull(revealButton);
                Assert.IsNotNull(nativePasswordBox);
                Assert.IsNotNull(revealTextBox);
                Assert.AreEqual(Visibility.Visible, revealButton.Visibility);
                Assert.AreEqual(string.Empty, nativePasswordBox.Password);
                Assert.AreEqual(
                    ((SolidColorBrush)passwordBox.FindResource("ZenTextSecondaryBrush")).Color,
                    ((SolidColorBrush)revealButton.Foreground).Color);

                nativePasswordBox.Password = "secret";
                passwordBox.IsPasswordRevealed = true;
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Collapsed, nativePasswordBox.Visibility);
                Assert.AreEqual(Visibility.Visible, revealTextBox.Visibility);
                Assert.AreEqual("secret", revealTextBox.Text);

                passwordBox.IsPasswordRevealed = false;
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Visible, nativePasswordBox.Visibility);
                Assert.AreEqual(Visibility.Collapsed, revealTextBox.Visibility);
                Assert.AreEqual(string.Empty, revealTextBox.Text);
                Assert.AreEqual("secret", nativePasswordBox.Password);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
