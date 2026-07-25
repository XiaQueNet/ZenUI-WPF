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
    public class SliderTests
    {
        [TestMethod]
        public void SliderSupportsVerticalOrientation()
        {
            var slider = new ZenSlider
            {
                Orientation = Orientation.Vertical,
                Height = 180,
                Value = 40
            };
            var window = CreateTestWindow(slider, 100, 220);
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

                var track = slider.Template.FindName("PART_Track", slider) as Track;
                Assert.IsNotNull(track);
                Assert.IsNotNull(track.DecreaseRepeatButton);
                Assert.AreEqual(Orientation.Vertical, track.Orientation);
                Assert.AreEqual(4d, track.DecreaseRepeatButton.Width);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                Assert.AreEqual(3d, track.DecreaseRepeatButton.Width);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();
                Assert.AreEqual(6d, track.DecreaseRepeatButton.Width);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
