using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class WatermarkFocusTests
    {
        [TestMethod]
        public void WatermarkControlsHonorShowWatermarkOnFocus()
        {
            var textBox = new ZenTextBox { Watermark = "文本" };
            var passwordBox = new ZenPasswordBox { Watermark = "密码" };
            var comboBox = new ZenComboBox
            {
                IsEditable = true,
                Watermark = "选项"
            };
            var datePicker = new ZenDatePicker { Watermark = "日期" };
            var timePicker = new ZenTimePicker { Watermark = "时间" };
            var dateTimePicker = new ZenDateTimePicker { Watermark = "日期时间" };
            var panel = new StackPanel();
            panel.Children.Add(textBox);
            panel.Children.Add(passwordBox);
            panel.Children.Add(comboBox);
            panel.Children.Add(datePicker);
            panel.Children.Add(timePicker);
            panel.Children.Add(dateTimePicker);
            var window = CreateTestWindow(panel, 300, 300);

            try
            {
                window.Show();
                window.UpdateLayout();

                var nativePasswordBox =
                    passwordBox.Template.FindName("PART_PasswordBox", passwordBox) as PasswordBox;
                var editableComboBox =
                    comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                var datePickerTextBox =
                    datePicker.Template.FindName("PART_TextBox", datePicker) as DatePickerTextBox;
                var timePickerTextBox =
                    timePicker.Template.FindName("PART_TextBox", timePicker) as TextBox;
                var dateTimePickerTextBox =
                    dateTimePicker.Template.FindName("PART_TextBox", dateTimePicker) as TextBox;
                Assert.IsNotNull(nativePasswordBox);
                Assert.IsNotNull(editableComboBox);
                Assert.IsNotNull(datePickerTextBox);
                Assert.IsNotNull(timePickerTextBox);
                Assert.IsNotNull(dateTimePickerTextBox);
                datePickerTextBox.ApplyTemplate();

                AssertFocusedWatermarkBehavior(
                    window,
                    textBox,
                    textBox,
                    GetWatermark(textBox, textBox),
                    value => textBox.ShowWatermarkOnFocus = value);
                AssertFocusedWatermarkBehavior(
                    window,
                    passwordBox,
                    nativePasswordBox,
                    GetWatermark(passwordBox, passwordBox),
                    value => passwordBox.ShowWatermarkOnFocus = value);
                AssertFocusedWatermarkBehavior(
                    window,
                    comboBox,
                    editableComboBox,
                    GetWatermark(comboBox, comboBox),
                    value => comboBox.ShowWatermarkOnFocus = value);
                AssertFocusedWatermarkBehavior(
                    window,
                    datePicker,
                    datePickerTextBox,
                    GetWatermark(datePickerTextBox, datePickerTextBox),
                    value => datePicker.ShowWatermarkOnFocus = value);
                AssertFocusedWatermarkBehavior(
                    window,
                    timePicker,
                    timePickerTextBox,
                    GetWatermark(timePicker, timePicker),
                    value => timePicker.ShowWatermarkOnFocus = value);
                AssertFocusedWatermarkBehavior(
                    window,
                    dateTimePicker,
                    dateTimePickerTextBox,
                    GetWatermark(dateTimePicker, dateTimePicker),
                    value => dateTimePicker.ShowWatermarkOnFocus = value);
            }
            finally
            {
                window.Close();
            }
        }

        private static TextBlock GetWatermark(Control templateOwner, FrameworkElement owner)
        {
            var watermark = templateOwner.Template.FindName("WatermarkText", owner) as TextBlock;
            Assert.IsNotNull(watermark);
            return watermark;
        }

        private static void AssertFocusedWatermarkBehavior(
            Window window,
            Control owner,
            Control focusTarget,
            TextBlock watermark,
            Action<bool> setShowWatermarkOnFocus)
        {
            focusTarget.Focus();
            window.UpdateLayout();

            Assert.IsTrue(owner.IsKeyboardFocusWithin);
            Assert.AreEqual(Visibility.Visible, watermark.Visibility);

            setShowWatermarkOnFocus(false);
            window.UpdateLayout();
            Assert.AreEqual(Visibility.Collapsed, watermark.Visibility);

            setShowWatermarkOnFocus(true);
            window.UpdateLayout();
            Assert.AreEqual(Visibility.Visible, watermark.Visibility);
        }
    }
}
