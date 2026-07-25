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
