using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
        [TestMethod]
        public void ThemesAndDpiScalesProduceReviewableVisualSnapshots()
        {
            var framework = Environment.Version.Major >= 8 ? "net8" : "net472";
            var outputDirectory = Path.Combine(AppContext.BaseDirectory, "visual-regression", framework);
            Directory.CreateDirectory(outputDirectory);
            var luminance = new Dictionary<ZenTheme, double>();

            foreach (var theme in new[] { ZenTheme.Light, ZenTheme.Dark, ZenTheme.HighContrast })
            {
                foreach (var scale in new[] { 1d, 1.5d, 2d })
                {
                    var root = CreateControlGallery(theme);
                    var bitmap = Render(root, scale);
                    var gallery = (StackPanel)root.Child;
                    var dataGrid = (ZenDataGrid)gallery.Children[gallery.Children.Count - 1];
                    Assert.IsGreaterThan(
                        50d,
                        dataGrid.Columns[0].ActualWidth,
                        $"Grid={dataGrid.ActualWidth}, second={dataGrid.Columns[1].ActualWidth}, desired={dataGrid.DesiredSize.Width}");
                    Assert.IsGreaterThan(20, CountDistinctSampledColors(bitmap));
                    SavePng(bitmap, Path.Combine(outputDirectory, $"{theme}-{scale:0.0}x.png"));
                    if (Math.Abs(scale - 1d) < 0.01d)
                    {
                        luminance[theme] = CalculateMeanLuminance(bitmap);
                    }
                }
            }

            Assert.IsGreaterThan(
                luminance[ZenTheme.Dark],
                luminance[ZenTheme.Light],
                "The light theme should remain perceptually brighter than the dark theme.");
        }

        private static Border CreateControlGallery(ZenTheme theme)
        {
            var root = new Border
            {
                Width = 540,
                Height = 450,
                Padding = new Thickness(20),
                FlowDirection = theme == ZenTheme.HighContrast ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
            };
            root.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            });
            ZenThemeManager.ApplyTheme(root.Resources, theme, false);
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
                Variant = AlertVariant.Success
            });

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
