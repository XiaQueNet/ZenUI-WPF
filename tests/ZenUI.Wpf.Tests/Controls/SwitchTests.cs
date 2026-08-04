using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class SwitchTests
    {
        [TestMethod]
        public void CapsuleTemplateUsesBrushBindingsFromDerivedStyle()
        {
            _ = new ZenSwitch();
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var palette = new SwitchBrushPalette();
            var style = new Style(typeof(ZenSwitch), (Style)resources["ZenSwitchStyle"]);
            style.Setters.Add(CreateBrushBindingSetter(
                Control.BackgroundProperty,
                nameof(SwitchBrushPalette.Track),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                Control.ForegroundProperty,
                nameof(SwitchBrushPalette.Thumb),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                Control.BorderBrushProperty,
                nameof(SwitchBrushPalette.ThumbBorder),
                palette));

            var @switch = new ZenSwitch
            {
                IsChecked = true,
                Style = style,
                Width = 80
            };
            var window = CreateTestWindow(@switch, 160, 90);
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                var track = @switch.Template.FindName("Track", @switch) as Border;
                var thumb = FindVisualDescendant<Ellipse>(@switch);
                Assert.IsNotNull(track);
                Assert.IsNotNull(thumb);
                Assert.AreEqual(
                    new CornerRadius(@switch.ActualHeight / 2d),
                    track.CornerRadius);
                Assert.IsTrue(
                    track.ActualWidth > track.CornerRadius.TopLeft * 2d,
                    "胶囊中间必须保留矩形段，不能退化为椭圆。");
                Assert.AreSame(palette.Track, @switch.Background);
                Assert.AreSame(palette.Track, track.Background);
                Assert.AreSame(palette.Thumb, thumb.Fill);
                Assert.AreSame(palette.ThumbBorder, thumb.Stroke);

                var updatedTrack = new SolidColorBrush(Colors.CadetBlue);
                palette.Track = updatedTrack;
                Assert.AreSame(updatedTrack, @switch.Background);
                Assert.AreSame(updatedTrack, track.Background);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void StateContentFollowsCheckedState()
        {
            var @switch = new ZenSwitch
            {
                CheckedContent = "开",
                UncheckedContent = "关"
            };
            var window = CreateTestWindow(@switch, 160, 90);
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

                var stateContent = @switch.Template.FindName(
                    "StateContent",
                    @switch) as ContentPresenter;
                var thumbHost = @switch.Template.FindName(
                    "ThumbHost",
                    @switch) as Grid;
                Assert.IsNotNull(stateContent);
                Assert.IsNotNull(thumbHost);
                Assert.AreEqual("关", stateContent.Content);
                Assert.AreEqual(
                    HorizontalAlignment.Center,
                    stateContent.HorizontalAlignment);
                Assert.AreEqual(1, Grid.GetColumn(stateContent));
                Assert.AreEqual(2, Grid.GetColumnSpan(stateContent));
                Assert.AreEqual(0, Grid.GetColumn(thumbHost));

                @switch.IsChecked = true;
                window.UpdateLayout();

                Assert.AreEqual("开", stateContent.Content);
                Assert.AreEqual(0, Grid.GetColumn(stateContent));
                Assert.AreEqual(2, Grid.GetColumnSpan(stateContent));
                Assert.AreEqual(2, Grid.GetColumn(thumbHost));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void CustomSizeStateContentKeepsThumbAtTrackEdges()
        {
            var @switch = new ZenSwitch
            {
                CheckedContent = "已开启",
                Height = 40,
                UncheckedContent = "已关闭",
                Width = 120
            };
            var window = CreateTestWindow(@switch, 180, 90);
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

                var track = @switch.Template.FindName("Track", @switch) as Border;
                var thumbHost = @switch.Template.FindName(
                    "ThumbHost",
                    @switch) as Grid;
                Assert.IsNotNull(track);
                Assert.IsNotNull(thumbHost);
                Assert.AreEqual(
                    0d,
                    thumbHost.TranslatePoint(new Point(), track).X,
                    0.01d);

                @switch.IsChecked = true;
                window.UpdateLayout();

                Assert.AreEqual(
                    track.ActualWidth - thumbHost.ActualWidth,
                    thumbHost.TranslatePoint(new Point(), track).X,
                    0.01d);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void HeightOnlyCustomizationPreservesTwoToOneMinimumWidth()
        {
            var @switch = new ZenSwitch
            {
                Height = 40
            };
            var window = CreateTestWindow(@switch, 160, 90);
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

                Assert.AreEqual(80d, @switch.MinWidth);
                Assert.AreEqual(80d, @switch.ActualWidth);
                Assert.AreEqual(40d, @switch.ActualHeight);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void StateContentIsOptional()
        {
            var @switch = new ZenSwitch();
            var window = CreateTestWindow(@switch, 160, 90);
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

                var stateContent = @switch.Template.FindName(
                    "StateContent",
                    @switch) as ContentPresenter;
                Assert.IsNotNull(stateContent);
                Assert.IsNull(stateContent.Content);

                @switch.IsChecked = true;
                window.UpdateLayout();

                Assert.IsNull(stateContent.Content);
            }
            finally
            {
                window.Close();
            }
        }

        private static Setter CreateBrushBindingSetter(
            DependencyProperty property,
            string path,
            object source)
        {
            return new Setter(
                property,
                new Binding(path)
                {
                    Source = source
                });
        }

        private sealed class SwitchBrushPalette : INotifyPropertyChanged
        {
            private Brush track = new SolidColorBrush(Colors.MediumPurple);

            public event PropertyChangedEventHandler PropertyChanged;

            public Brush Track
            {
                get { return track; }
                set
                {
                    track = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Track)));
                }
            }

            public Brush Thumb { get; } = new SolidColorBrush(Colors.WhiteSmoke);
            public Brush ThumbBorder { get; } = new SolidColorBrush(Colors.Indigo);
        }
    }
}
