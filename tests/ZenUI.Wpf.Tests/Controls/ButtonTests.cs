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
    }
}
