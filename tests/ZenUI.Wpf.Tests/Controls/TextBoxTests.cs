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
    public class TextBoxTests
    {
        [TestMethod]
        public void TextBoxTemplateAppliesWatermarkAndCornerRadius()
        {
            var textBox = new ZenTextBox
            {
                Watermark = "请输入内容",
                CornerRadius = new CornerRadius(12)
            };
            var window = CreateTestWindow(textBox, 200, 100);

            try
            {
                window.Show();
                window.UpdateLayout();

                var inputBorder = textBox.Template.FindName("InputBorder", textBox) as Border;
                var watermark = textBox.Template.FindName("WatermarkText", textBox) as TextBlock;
                var watermarkHost = textBox.Template.FindName("WatermarkHost", textBox) as Border;
                Assert.IsNotNull(inputBorder);
                Assert.IsNotNull(watermark);
                Assert.IsNotNull(watermarkHost);
                Assert.AreEqual(new CornerRadius(12), inputBorder.CornerRadius);
                Assert.AreEqual("请输入内容", watermark.Text);
                Assert.AreEqual(textBox.Padding, watermarkHost.Margin);
                Assert.AreEqual(new Thickness(), watermark.Margin);
                Assert.AreEqual(Visibility.Visible, watermark.Visibility);

                textBox.Text = "ZenUI";
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Collapsed, watermark.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void TextInputDisplaysValidationErrors()
        {
            var textBox = new ZenTextBox();
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(InvalidModel.Value))
            {
                Source = new InvalidModel(),
                ValidatesOnDataErrors = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 240,
                Height = 100,
                Content = textBox
            };

            try
            {
                window.Show();
                textBox.Text = "invalid";
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                window.UpdateLayout();

                Assert.IsTrue(Validation.GetHasError(textBox));
                var inputBorder = textBox.Template.FindName("InputBorder", textBox) as Border;
                Assert.IsNotNull(inputBorder);
                Assert.AreEqual(new Thickness(2), inputBorder.BorderThickness);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
