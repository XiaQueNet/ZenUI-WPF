using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class StyleBrushBindingTests
    {
        [TestMethod]
        public void CoreControlTemplatesUseBrushBindingsFromDerivedStyles()
        {
            _ = new ZenAlert();
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var palette = new ControlBrushPalette();
            var alert = CreateAlert(resources, palette);
            var checkBox = CreateCheckBox(resources, palette);
            var radioButton = CreateRadioButton(resources, palette);
            var slider = CreateSlider(resources, palette);
            var scrollBar = CreateScrollBar(resources, palette);
            var panel = new StackPanel();
            panel.Children.Add(alert);
            panel.Children.Add(checkBox);
            panel.Children.Add(radioButton);
            panel.Children.Add(slider);
            panel.Children.Add(scrollBar);
            var window = CreateTestWindow(panel, 360, 360);
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                AssertAlertBrushes(alert, palette);
                AssertCheckBoxBrushes(checkBox, palette);
                AssertRadioButtonBrushes(radioButton, palette);
                AssertSliderBrushes(slider, palette);
                AssertScrollBarBrushes(scrollBar, palette);

                var updatedSurface = new SolidColorBrush(Colors.CadetBlue);
                palette.Surface = updatedSurface;
                Assert.AreSame(
                    updatedSurface,
                    ((Border)alert.Template.FindName("AlertBorder", alert)).Background);
                Assert.AreSame(
                    updatedSurface,
                    ((Border)checkBox.Template.FindName("Box", checkBox)).Background);
                Assert.AreSame(
                    updatedSurface,
                    ((Ellipse)radioButton.Template.FindName("Ring", radioButton)).Fill);
                Assert.AreSame(
                    updatedSurface,
                    ((Track)slider.Template.FindName("PART_Track", slider))
                        .IncreaseRepeatButton.Background);
                Assert.AreSame(
                    updatedSurface,
                    ((Border)scrollBar.Template.FindName("TrackBackground", scrollBar))
                        .Background);
            }
            finally
            {
                window.Close();
            }
        }

        private static ZenAlert CreateAlert(
            ResourceDictionary resources,
            ControlBrushPalette palette)
        {
            var style = CreateStyle(resources, "ZenAlertStyle", typeof(ZenAlert));
            AddBrushBinding(style, Control.BackgroundProperty, nameof(palette.Surface), palette);
            AddBrushBinding(style, Control.BorderBrushProperty, nameof(palette.Border), palette);
            AddBrushBinding(style, Control.ForegroundProperty, nameof(palette.Foreground), palette);
            AddBrushBinding(style, ZenAlert.AccentBrushProperty, nameof(palette.Accent), palette);
            AddBrushBinding(style, ZenAlert.IconForegroundProperty, nameof(palette.Glyph), palette);
            return new ZenAlert
            {
                Content = "提示",
                Style = style,
                Severity = AlertSeverity.Error
            };
        }

        private static ZenCheckBox CreateCheckBox(
            ResourceDictionary resources,
            ControlBrushPalette palette)
        {
            var style = CreateStyle(resources, "ZenCheckBoxStyle", typeof(ZenCheckBox));
            AddBrushBinding(style, Control.BackgroundProperty, nameof(palette.Surface), palette);
            AddBrushBinding(style, Control.BorderBrushProperty, nameof(palette.Border), palette);
            AddBrushBinding(style, Control.ForegroundProperty, nameof(palette.Foreground), palette);
            AddBrushBinding(style, ZenCheckBox.CheckedBackgroundProperty, nameof(palette.Accent), palette);
            AddBrushBinding(style, ZenCheckBox.CheckedBorderBrushProperty, nameof(palette.Accent), palette);
            AddBrushBinding(style, ZenCheckBox.HoverBorderBrushProperty, nameof(palette.Accent), palette);
            AddBrushBinding(style, ZenCheckBox.CheckMarkForegroundProperty, nameof(palette.Glyph), palette);
            return new ZenCheckBox
            {
                Content = "复选",
                IsChecked = true,
                Style = style
            };
        }

        private static ZenRadioButton CreateRadioButton(
            ResourceDictionary resources,
            ControlBrushPalette palette)
        {
            var style = CreateStyle(
                resources,
                "ZenRadioButtonStyle",
                typeof(ZenRadioButton));
            AddBrushBinding(style, Control.BackgroundProperty, nameof(palette.Surface), palette);
            AddBrushBinding(style, Control.BorderBrushProperty, nameof(palette.Border), palette);
            AddBrushBinding(style, Control.ForegroundProperty, nameof(palette.Foreground), palette);
            AddBrushBinding(style, ZenRadioButton.AccentBrushProperty, nameof(palette.Accent), palette);
            return new ZenRadioButton
            {
                Content = "单选",
                IsChecked = true,
                Style = style
            };
        }

        private static ZenSlider CreateSlider(
            ResourceDictionary resources,
            ControlBrushPalette palette)
        {
            var style = CreateStyle(resources, "ZenSliderStyle", typeof(ZenSlider));
            AddBrushBinding(style, Control.BackgroundProperty, nameof(palette.Surface), palette);
            AddBrushBinding(style, Control.ForegroundProperty, nameof(palette.Foreground), palette);
            AddBrushBinding(style, Control.BorderBrushProperty, nameof(palette.Border), palette);
            AddBrushBinding(style, ZenSlider.ThumbBrushProperty, nameof(palette.Thumb), palette);
            AddBrushBinding(
                style,
                ZenSlider.ThumbHoverBrushProperty,
                nameof(palette.ThumbHover),
                palette);
            return new ZenSlider
            {
                Style = style,
                Value = 50,
                Width = 240
            };
        }

        private static ScrollBar CreateScrollBar(
            ResourceDictionary resources,
            ControlBrushPalette palette)
        {
            var style = CreateStyle(resources, "ZenScrollBarStyle", typeof(ScrollBar));
            AddBrushBinding(style, Control.BackgroundProperty, nameof(palette.Surface), palette);
            AddBrushBinding(style, Control.ForegroundProperty, nameof(palette.Foreground), palette);
            return new ScrollBar
            {
                Height = 120,
                Maximum = 100,
                Style = style,
                ViewportSize = 10
            };
        }

        private static void AssertAlertBrushes(
            ZenAlert alert,
            ControlBrushPalette palette)
        {
            var border = (Border)alert.Template.FindName("AlertBorder", alert);
            var icon = (Ellipse)alert.Template.FindName("Icon", alert);
            var iconText = (TextBlock)alert.Template.FindName("IconText", alert);
            Assert.AreSame(palette.Surface, border.Background);
            Assert.AreSame(palette.Border, border.BorderBrush);
            Assert.AreSame(palette.Accent, icon.Fill);
            Assert.AreSame(palette.Glyph, iconText.Foreground);
            Assert.AreSame(palette.Foreground, alert.Foreground);
        }

        private static void AssertCheckBoxBrushes(
            ZenCheckBox checkBox,
            ControlBrushPalette palette)
        {
            var box = (Border)checkBox.Template.FindName("Box", checkBox);
            var glyph = (Path)checkBox.Template.FindName("CheckMark", checkBox);
            Assert.AreSame(palette.Surface, box.Background);
            Assert.AreSame(palette.Border, box.BorderBrush);
            Assert.AreSame(palette.Glyph, glyph.Stroke);
            Assert.AreSame(palette.Foreground, checkBox.Foreground);
        }

        private static void AssertRadioButtonBrushes(
            ZenRadioButton radioButton,
            ControlBrushPalette palette)
        {
            var ring = (Ellipse)radioButton.Template.FindName("Ring", radioButton);
            var dot = (Ellipse)radioButton.Template.FindName("Dot", radioButton);
            Assert.AreSame(palette.Surface, ring.Fill);
            Assert.AreSame(palette.Border, ring.Stroke);
            Assert.AreSame(palette.Accent, dot.Fill);
            Assert.AreSame(palette.Foreground, radioButton.Foreground);
        }

        private static void AssertSliderBrushes(
            ZenSlider slider,
            ControlBrushPalette palette)
        {
            var track = (Track)slider.Template.FindName("PART_Track", slider);
            track.Thumb.ApplyTemplate();
            var thumb = (Ellipse)track.Thumb.Template.FindName("ThumbShape", track.Thumb);
            Assert.AreSame(palette.Foreground, track.DecreaseRepeatButton.Background);
            Assert.AreSame(palette.Surface, track.IncreaseRepeatButton.Background);
            Assert.AreSame(palette.Thumb, thumb.Fill);
            Assert.AreSame(palette.Border, thumb.Stroke);
        }

        private static void AssertScrollBarBrushes(
            ScrollBar scrollBar,
            ControlBrushPalette palette)
        {
            var trackBackground =
                (Border)scrollBar.Template.FindName("TrackBackground", scrollBar);
            var track = (Track)scrollBar.Template.FindName("PART_Track", scrollBar);
            track.Thumb.ApplyTemplate();
            var thumb =
                (Border)track.Thumb.Template.FindName("ThumbShape", track.Thumb);
            Assert.AreSame(palette.Surface, trackBackground.Background);
            Assert.AreSame(palette.Foreground, thumb.Background);
        }

        private static Style CreateStyle(
            ResourceDictionary resources,
            string key,
            Type targetType)
        {
            return new Style(targetType, (Style)resources[key]);
        }

        private static void AddBrushBinding(
            Style style,
            DependencyProperty property,
            string path,
            object source)
        {
            style.Setters.Add(
                new Setter(
                    property,
                    new Binding(path)
                    {
                        Source = source
                    }));
        }

        private sealed class ControlBrushPalette : INotifyPropertyChanged
        {
            private Brush surface = new SolidColorBrush(Colors.MediumPurple);

            public event PropertyChangedEventHandler PropertyChanged;

            public Brush Surface
            {
                get { return surface; }
                set
                {
                    surface = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Surface)));
                }
            }

            public Brush Foreground { get; } = new SolidColorBrush(Colors.WhiteSmoke);
            public Brush Border { get; } = new SolidColorBrush(Colors.Indigo);
            public Brush Accent { get; } = new SolidColorBrush(Colors.DarkOrange);
            public Brush Glyph { get; } = new SolidColorBrush(Colors.White);
            public Brush Thumb { get; } = new SolidColorBrush(Colors.MistyRose);
            public Brush ThumbHover { get; } = new SolidColorBrush(Colors.Plum);
        }
    }
}
