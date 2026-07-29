using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class VisualRegressionTests
    {
        private static readonly Dictionary<ZenTheme, string> LuminanceBaselines =
            new Dictionary<ZenTheme, string>
            {
                [ZenTheme.Light] = "9Onr4+rr6OX2/////////+a53+bCzvDw+f/////////mweLxv9T//////////////uHm/f39/f39/f39/f39/vny/v7+/v7+/v7+/v7+/P7+8/b+/v7+/v7+/v7+/vn+8+Hg5OTk5OTk5Pj6+vr1/Pjp7OHo6Ozv7+/5+vr6+v386ePi4tjz+fn5+fn5+fn8/OHg8/Pz8/Pz8/Pz8/P18vzj3fX19fX19fX19fX19/X+/f39/f39/f39/f39/f7+/Of4+Pj4+Pj4+Pjy6vj5//7r+/39/f39/f399+/9/vb/+/3///////////z4///5/+P4///////////57v///g==",
                [ZenTheme.Dark] = "LjM0OTIzNjkpISEhISEhITpjQTZSZjAuJSEhISEhISE7aT4wU2khISEhISEhISEhIj45IyMjIyMjIyMjIyMjIictIiIiIiIiIiIiIiIiJCIiLSojIyMjIyMjIyMjIyciKjk7Nzc3Nzc3NykoKCgtJSk8NTI0NDU1NTUuLi4uLiYoQCwrKykvMDAwMDAwMDAoJ0pLOjo6Ojo6Ojo6Ojo1LidITTg4ODg4ODg4ODg4MiwiJSUlJSUlJSUlJSUlJSQjJTwrKysrKysrKysxOSspISI0JiMjIyMjIyMjKTEjIyshJSIhISEhISEhISMnISEoITsnISEhISEhISEmMSEhIw=="
            };

        [TestMethod]
        public void ThemesDensitiesAndDpiScalesProduceReviewableVisualSnapshots()
        {
            var framework = Environment.Version.Major >= 8 ? "net8" : "net472";
            var outputDirectory = Path.Combine(AppContext.BaseDirectory, "visual-regression", framework);
            Directory.CreateDirectory(outputDirectory);
            var luminance = new Dictionary<ZenTheme, double>();

            foreach (var theme in new[] { ZenTheme.Light, ZenTheme.Dark, ZenTheme.HighContrast })
            {
                foreach (var density in new[] { ZenDensity.Compact, ZenDensity.Standard, ZenDensity.Comfortable })
                {
                    foreach (var scale in new[] { 1.25d, 1.5d, 2d })
                    {
                        var root = CreateControlGallery(theme, density);
                        var bitmap = Render(root, scale);
                        var gallery = (StackPanel)root.Child;
                        var dataGrid = (ZenDataGrid)gallery.Children[gallery.Children.Count - 1];
                        var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                        Assert.IsNotNull(row);
                        Assert.AreEqual(
                            (double)root.Resources["ZenDataGridRowMinHeight"],
                            row.MinHeight);
                        Assert.IsGreaterThan(
                            50d,
                            dataGrid.Columns[0].ActualWidth,
                            $"Grid={dataGrid.ActualWidth}, second={dataGrid.Columns[1].ActualWidth}, desired={dataGrid.DesiredSize.Width}");
                        Assert.IsGreaterThan(20, CountDistinctSampledColors(bitmap));
                        if (density == ZenDensity.Standard &&
                            LuminanceBaselines.TryGetValue(theme, out var encodedBaseline))
                        {
                            var difference = CalculateMeanAbsoluteDifference(
                                Convert.FromBase64String(encodedBaseline),
                                CalculateNormalizedLuminanceFingerprint(bitmap));
                            Assert.IsLessThan(
                                12d,
                                difference,
                                $"{theme} at {scale:0.00}x differs materially from its approved visual baseline.");
                        }

                        SavePng(
                            bitmap,
                            Path.Combine(outputDirectory, $"{theme}-{density}-{scale:0.00}x.png"));
                        if (density == ZenDensity.Standard && Math.Abs(scale - 1.5d) < 0.01d)
                        {
                            luminance[theme] = CalculateMeanLuminance(bitmap);
                        }
                    }
                }
            }

            Assert.IsGreaterThan(
                luminance[ZenTheme.Dark],
                luminance[ZenTheme.Light],
                "The light theme should remain perceptually brighter than the dark theme.");
        }

        [TestMethod]
        public void CalendarPopupThemesAndDensitiesProduceReviewableVisualSnapshots()
        {
            var framework = Environment.Version.Major >= 8 ? "net8" : "net472";
            var outputDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "visual-regression",
                framework,
                "calendar");
            Directory.CreateDirectory(outputDirectory);

            foreach (var theme in new[] { ZenTheme.Light, ZenTheme.Dark, ZenTheme.HighContrast })
            {
                foreach (var density in new[] { ZenDensity.Compact, ZenDensity.Standard, ZenDensity.Comfortable })
                {
                    var datePicker = new ZenDatePicker
                    {
                        FlowDirection = theme == ZenTheme.HighContrast
                            ? FlowDirection.RightToLeft
                            : FlowDirection.LeftToRight,
                        SelectedDate = new DateTime(2026, 7, 23)
                    };
                    var window = new Window
                    {
                        ShowInTaskbar = false,
                        WindowStyle = WindowStyle.None,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 360,
                        Height = 420,
                        Content = datePicker
                    };
                    window.Resources.MergedDictionaries.Add(new ResourceDictionary
                    {
                        Source = new Uri(
                            "/ZenUI.Wpf;component/Themes/Generic.xaml",
                            UriKind.Relative)
                    });
                    ZenThemeManager.ApplyTheme(window.Resources, theme, false);
                    ZenDensityManager.ApplyDensity(window.Resources, density);

                    try
                    {
                        window.Show();
                        datePicker.IsDropDownOpen = true;
                        window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                        window.UpdateLayout();

                        var calendar = ControlTestHelper.GetDatePickerCalendar(datePicker);
                        Assert.IsNotNull(calendar);
                        calendar.ApplyTemplate();
                        var calendarItem =
                            calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;
                        Assert.IsNotNull(calendarItem);
                        calendarItem.ApplyTemplate();
                        window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                        window.UpdateLayout();

                        var monthView =
                            calendarItem.Template.FindName("PART_MonthView", calendarItem) as Grid;
                        Assert.IsNotNull(monthView);
                        var dayButton = FindCalendarDayButton(monthView, calendar.CalendarDayButtonStyle);
                        Assert.IsNotNull(dayButton);
                        Assert.IsTrue(double.IsNaN(dayButton.Width));
                        Assert.IsTrue(double.IsNaN(dayButton.Height));
                        Assert.IsGreaterThan(0d, dayButton.ActualWidth);
                        Assert.IsGreaterThan(0d, dayButton.ActualHeight);

                        var bitmap = RenderRealizedElement(calendar, 1.25d);
                        Assert.IsGreaterThan(12, CountDistinctSampledColors(bitmap));
                        SavePng(
                            bitmap,
                            Path.Combine(outputDirectory, $"{theme}-{density}-1.25x.png"));
                    }
                    finally
                    {
                        datePicker.IsDropDownOpen = false;
                        window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                        window.Close();
                    }
                }
            }
        }

        private static Border CreateControlGallery(ZenTheme theme, ZenDensity density)
        {
            var root = new Border
            {
                Width = 540,
                Height = 570,
                Padding = new Thickness(20),
                FlowDirection = theme == ZenTheme.HighContrast ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
            };
            root.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            });
            ZenThemeManager.ApplyTheme(root.Resources, theme, false);
            ZenDensityManager.ApplyDensity(root.Resources, density);
            root.SetResourceReference(Border.BackgroundProperty, "ZenSurfaceBrush");

            var panel = new StackPanel();
            root.Child = panel;
            var title = new TextBlock
            {
                Text = theme + " · ZenUI visual snapshot",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 14)
            };
            title.SetResourceReference(TextBlock.ForegroundProperty, "ZenTextPrimaryBrush");
            panel.Children.Add(title);

            var actions = new StackPanel { Orientation = Orientation.Horizontal };
            actions.Children.Add(new ZenButton { Content = "主要操作", Margin = new Thickness(0, 0, 10, 0) });
            actions.Children.Add(new ZenButton { Content = "禁用", IsEnabled = false });
            actions.Children.Add(new ZenSwitch { Margin = new Thickness(16, 0, 0, 0), IsChecked = true });
            panel.Children.Add(actions);
            panel.Children.Add(new ZenTextBox
            {
                Margin = new Thickness(0, 14, 0, 0),
                Text = "可编辑文本",
                Watermark = "请输入内容"
            });
            var comboBox = new ZenComboBox
            {
                Margin = new Thickness(0, 10, 0, 0),
                Watermark = "请选择"
            };
            comboBox.Items.Add("第一项");
            comboBox.Items.Add("第二项");
            comboBox.SelectedIndex = 0;
            panel.Children.Add(comboBox);
            panel.Children.Add(new ZenDatePicker
            {
                Margin = new Thickness(0, 10, 0, 0),
                SelectedDate = new DateTime(2026, 7, 23)
            });
            panel.Children.Add(new ZenProgressBar
            {
                Margin = new Thickness(0, 14, 0, 0),
                Maximum = 100,
                Value = 64
            });
            panel.Children.Add(new ZenAlert
            {
                Margin = new Thickness(0, 14, 0, 0),
                Content = "主题、焦点和语义颜色快照",
                Severity = AlertSeverity.Success
            });

            var listBox = new ZenListBox
            {
                Height = 92,
                Margin = new Thickness(0, 14, 0, 0),
                SelectionMode = SelectionMode.Extended
            };
            listBox.Items.Add("列表项目一");
            listBox.Items.Add("列表项目二");
            listBox.Items.Add("列表项目三");
            listBox.SelectedItems.Add(listBox.Items[0]);
            listBox.SelectedItems.Add(listBox.Items[1]);
            panel.Children.Add(listBox);

            var dataGrid = new ZenDataGrid
            {
                Width = 500,
                Height = 125,
                Margin = new Thickness(0, 14, 0, 0),
                AutoGenerateColumns = false,
                IsReadOnly = true,
                ItemsSource = new[]
                {
                    new SnapshotRow("林知夏", "在线"),
                    new SnapshotRow("周景明", "忙碌")
                }
            };
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "姓名",
                Binding = new System.Windows.Data.Binding(nameof(SnapshotRow.Name))
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 120,
                Header = "状态",
                Binding = new System.Windows.Data.Binding(nameof(SnapshotRow.Status))
            });
            panel.Children.Add(dataGrid);
            return root;
        }

        private static CalendarDayButton FindCalendarDayButton(Grid monthView, Style dayButtonStyle)
        {
            foreach (var child in monthView.Children)
            {
                if (child is CalendarDayButton button &&
                    button.Visibility == Visibility.Visible &&
                    ReferenceEquals(button.Style, dayButtonStyle))
                {
                    return button;
                }
            }

            return null;
        }

        private static RenderTargetBitmap Render(FrameworkElement root, double scale)
        {
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Width = root.Width,
                Height = root.Height,
                Content = root
            };

            try
            {
                window.Show();
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                root.UpdateLayout();
                var bitmap = new RenderTargetBitmap(
                    (int)Math.Ceiling(root.ActualWidth * scale),
                    (int)Math.Ceiling(root.ActualHeight * scale),
                    96d * scale,
                    96d * scale,
                    PixelFormats.Pbgra32);
                bitmap.Render(root);
                bitmap.Freeze();
                return bitmap;
            }
            finally
            {
                window.Close();
            }
        }

        private static RenderTargetBitmap RenderRealizedElement(FrameworkElement element, double scale)
        {
            element.UpdateLayout();
            var width = Math.Max(element.ActualWidth, element.DesiredSize.Width);
            var height = Math.Max(element.ActualHeight, element.DesiredSize.Height);
            if (width <= 0d || height <= 0d)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                width = element.DesiredSize.Width;
                height = element.DesiredSize.Height;
                element.Arrange(new Rect(0d, 0d, width, height));
                element.UpdateLayout();
            }

            Assert.IsGreaterThan(0d, width);
            Assert.IsGreaterThan(0d, height);
            var bitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(width * scale),
                (int)Math.Ceiling(height * scale),
                96d * scale,
                96d * scale,
                PixelFormats.Pbgra32);
            bitmap.Render(element);
            bitmap.Freeze();
            return bitmap;
        }

        private static int CountDistinctSampledColors(BitmapSource bitmap)
        {
            var pixels = CopyPixels(bitmap);
            var colors = new HashSet<int>();
            for (var index = 0; index < pixels.Length; index += 68)
            {
                colors.Add(BitConverter.ToInt32(pixels, index - index % 4));
            }

            return colors.Count;
        }

        private static double CalculateMeanLuminance(BitmapSource bitmap)
        {
            var pixels = CopyPixels(bitmap);
            double total = 0;
            var count = 0;
            for (var index = 0; index < pixels.Length; index += 16)
            {
                total += 0.0722d * pixels[index] + 0.7152d * pixels[index + 1] + 0.2126d * pixels[index + 2];
                count++;
            }

            return total / count;
        }

        private static byte[] CalculateNormalizedLuminanceFingerprint(RenderTargetBitmap bitmap)
        {
            const int gridSize = 16;
            var pixels = CopyPixels(bitmap);
            var stride = bitmap.PixelWidth * 4;
            var fingerprint = new byte[gridSize * gridSize];
            var fingerprintIndex = 0;

            for (var gridY = 0; gridY < gridSize; gridY++)
            {
                var top = gridY * bitmap.PixelHeight / gridSize;
                var bottom = Math.Max(top + 1, (gridY + 1) * bitmap.PixelHeight / gridSize);
                for (var gridX = 0; gridX < gridSize; gridX++)
                {
                    var left = gridX * bitmap.PixelWidth / gridSize;
                    var right = Math.Max(left + 1, (gridX + 1) * bitmap.PixelWidth / gridSize);
                    double total = 0d;
                    var count = 0;

                    for (var y = top; y < bottom; y += 3)
                    {
                        for (var x = left; x < right; x += 3)
                        {
                            var pixelIndex = y * stride + x * 4;
                            total +=
                                0.0722d * pixels[pixelIndex] +
                                0.7152d * pixels[pixelIndex + 1] +
                                0.2126d * pixels[pixelIndex + 2];
                            count++;
                        }
                    }

                    fingerprint[fingerprintIndex++] = (byte)Math.Round(total / count);
                }
            }

            return fingerprint;
        }

        private static double CalculateMeanAbsoluteDifference(byte[] expected, byte[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            double difference = 0d;
            for (var index = 0; index < expected.Length; index++)
            {
                difference += Math.Abs(expected[index] - actual[index]);
            }

            return difference / expected.Length;
        }

        private static byte[] CopyPixels(BitmapSource bitmap)
        {
            var stride = bitmap.PixelWidth * 4;
            var pixels = new byte[stride * bitmap.PixelHeight];
            bitmap.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        private static void SavePng(BitmapSource bitmap, string path)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = File.Create(path))
            {
                encoder.Save(stream);
            }
        }

        private sealed class SnapshotRow
        {
            public SnapshotRow(string name, string status)
            {
                Name = name;
                Status = status;
            }

            public string Name { get; }

            public string Status { get; }
        }
    }
}
