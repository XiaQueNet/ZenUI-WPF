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
    public class ButtonTests
    {
        [TestMethod]
        public void NeutralVariantUsesNeutralThemeBrushesForEveryAppearance()
        {
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var filled = new ZenButton { Variant = ButtonVariant.Neutral };
            var outlined = new ZenButton
            {
                Appearance = ButtonAppearance.Outlined,
                Variant = ButtonVariant.Neutral
            };
            var text = new ZenButton
            {
                Appearance = ButtonAppearance.Text,
                Variant = ButtonVariant.Neutral
            };
            var panel = new StackPanel();
            panel.Children.Add(filled);
            panel.Children.Add(outlined);
            panel.Children.Add(text);
            var window = CreateTestWindow(panel, 320, 180);
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreEqual(ButtonVariant.Neutral, filled.Variant);
                Assert.AreSame(resources["ZenNeutralActionBrush"], filled.ThemeBackground);
                Assert.AreSame(resources["ZenOnNeutralActionBrush"], filled.ThemeForeground);
                Assert.AreSame(resources["ZenNeutralActionHoverBrush"], filled.ThemeHoverBackground);
                Assert.AreSame(resources["ZenNeutralActionPressedBrush"], filled.ThemePressedBackground);

                Assert.AreEqual(Brushes.Transparent, outlined.ThemeBackground);
                Assert.AreSame(resources["ZenNeutralActionBrush"], outlined.ThemeBorderBrush);
                Assert.AreSame(resources["ZenNeutralActionBrush"], outlined.ThemeForeground);
                Assert.AreSame(resources["ZenNeutralActionHoverBrush"], outlined.ThemeHoverBorderBrush);
                Assert.AreSame(resources["ZenNeutralActionPressedBrush"], outlined.ThemePressedBorderBrush);

                Assert.AreEqual(Brushes.Transparent, text.ThemeBackground);
                Assert.AreSame(resources["ZenNeutralActionBrush"], text.ThemeForeground);
                Assert.AreSame(resources["ZenSurfaceMutedBrush"], text.ThemeHoverBackground);
                Assert.AreSame(resources["ZenNeutralActionHoverBrush"], text.ThemeHoverForeground);
                Assert.AreSame(resources["ZenSurfaceDisabledBrush"], text.ThemePressedBackground);
                Assert.AreSame(resources["ZenNeutralActionPressedBrush"], text.ThemePressedForeground);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void TextAppearanceUsesVariantInteractionBackgrounds()
        {
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var primary = new ZenButton { Appearance = ButtonAppearance.Text };
            var success = new ZenButton
            {
                Appearance = ButtonAppearance.Text,
                Variant = ButtonVariant.Success
            };
            var warning = new ZenButton
            {
                Appearance = ButtonAppearance.Text,
                Variant = ButtonVariant.Warning
            };
            var panel = new StackPanel();
            panel.Children.Add(primary);
            panel.Children.Add(success);
            panel.Children.Add(warning);
            var window = CreateTestWindow(panel, 320, 180);
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreSame(resources["ZenInfoLightBrush"], primary.ThemeHoverBackground);
                Assert.AreSame(resources["ZenInfoLightBrush"], primary.ThemePressedBackground);
                Assert.AreSame(resources["ZenSuccessLightBrush"], success.ThemeHoverBackground);
                Assert.AreSame(resources["ZenSuccessLightBrush"], success.ThemePressedBackground);
                Assert.AreSame(resources["ZenWarningLightBrush"], warning.ThemeHoverBackground);
                Assert.AreSame(resources["ZenWarningLightBrush"], warning.ThemePressedBackground);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void ButtonInteractionBrushesAreCustomizable()
        {
            var button = new ZenButton();
            var hoverBackground = new SolidColorBrush(Colors.AliceBlue);
            var pressedBackground = new SolidColorBrush(Colors.LightBlue);
            var hoverForeground = new SolidColorBrush(Colors.Navy);
            var pressedForeground = new SolidColorBrush(Colors.DarkBlue);
            var hoverBorderBrush = new SolidColorBrush(Colors.CornflowerBlue);
            var pressedBorderBrush = new SolidColorBrush(Colors.RoyalBlue);

            button.HoverBackground = hoverBackground;
            button.PressedBackground = pressedBackground;
            button.HoverForeground = hoverForeground;
            button.PressedForeground = pressedForeground;
            button.HoverBorderBrush = hoverBorderBrush;
            button.PressedBorderBrush = pressedBorderBrush;

            Assert.AreSame(hoverBackground, button.HoverBackground);
            Assert.AreSame(pressedBackground, button.PressedBackground);
            Assert.AreSame(hoverForeground, button.HoverForeground);
            Assert.AreSame(pressedForeground, button.PressedForeground);
            Assert.AreSame(hoverBorderBrush, button.HoverBorderBrush);
            Assert.AreSame(pressedBorderBrush, button.PressedBorderBrush);
        }

        [TestMethod]
        public void DerivedStyleBrushBindingsOverrideSemanticThemeDefaults()
        {
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var palette = new ButtonBrushPalette();
            var style = new Style(typeof(ZenButton), (Style)resources["ZenButtonStyle"]);
            style.Setters.Add(CreateBrushBindingSetter(
                Control.BackgroundProperty,
                nameof(ButtonBrushPalette.Background),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                Control.ForegroundProperty,
                nameof(ButtonBrushPalette.Foreground),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                Control.BorderBrushProperty,
                nameof(ButtonBrushPalette.BorderBrush),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                ZenButton.HoverBackgroundProperty,
                nameof(ButtonBrushPalette.HoverBackground),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                ZenButton.HoverForegroundProperty,
                nameof(ButtonBrushPalette.HoverForeground),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                ZenButton.HoverBorderBrushProperty,
                nameof(ButtonBrushPalette.HoverBorderBrush),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                ZenButton.PressedBackgroundProperty,
                nameof(ButtonBrushPalette.PressedBackground),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                ZenButton.PressedForegroundProperty,
                nameof(ButtonBrushPalette.PressedForeground),
                palette));
            style.Setters.Add(CreateBrushBindingSetter(
                ZenButton.PressedBorderBrushProperty,
                nameof(ButtonBrushPalette.PressedBorderBrush),
                palette));

            var button = new ZenButton
            {
                Appearance = ButtonAppearance.Outlined,
                Style = style,
                Variant = ButtonVariant.Warning
            };
            var window = CreateTestWindow(button, 240, 100);
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreSame(palette.Background, button.Background);
                Assert.AreSame(palette.Foreground, button.Foreground);
                Assert.AreSame(palette.BorderBrush, button.BorderBrush);
                Assert.AreSame(palette.HoverBackground, button.HoverBackground);
                Assert.AreSame(palette.HoverForeground, button.HoverForeground);
                Assert.AreSame(palette.HoverBorderBrush, button.HoverBorderBrush);
                Assert.AreSame(palette.PressedBackground, button.PressedBackground);
                Assert.AreSame(palette.PressedForeground, button.PressedForeground);
                Assert.AreSame(palette.PressedBorderBrush, button.PressedBorderBrush);

                var updatedBackground = new SolidColorBrush(Colors.CadetBlue);
                palette.Background = updatedBackground;
                Assert.AreSame(updatedBackground, button.Background);
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

        private sealed class ButtonBrushPalette : INotifyPropertyChanged
        {
            private Brush background = new SolidColorBrush(Colors.MediumPurple);

            public event PropertyChangedEventHandler PropertyChanged;

            public Brush Background
            {
                get { return background; }
                set
                {
                    background = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Background)));
                }
            }

            public Brush Foreground { get; } = new SolidColorBrush(Colors.WhiteSmoke);
            public Brush BorderBrush { get; } = new SolidColorBrush(Colors.Indigo);
            public Brush HoverBackground { get; } = new SolidColorBrush(Colors.Plum);
            public Brush HoverForeground { get; } = new SolidColorBrush(Colors.MidnightBlue);
            public Brush HoverBorderBrush { get; } = new SolidColorBrush(Colors.DarkOrchid);
            public Brush PressedBackground { get; } = new SolidColorBrush(Colors.Thistle);
            public Brush PressedForeground { get; } = new SolidColorBrush(Colors.DarkSlateBlue);
            public Brush PressedBorderBrush { get; } = new SolidColorBrush(Colors.BlueViolet);
        }
    }
}
