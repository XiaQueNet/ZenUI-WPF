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

namespace ZenUI.Wpf.Tests.Controls
{
    internal static class ControlTestHelper
    {
        internal static ZenDataGrid CreateAdvancedDataGrid(
            IList<EditableRow> rows,
            out DataGridTextColumn nameColumn)
        {
            nameColumn = new DataGridTextColumn
            {
                Header = "姓名",
                SortMemberPath = nameof(EditableRow.Name),
                Binding = new Binding(nameof(EditableRow.Name))
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            };
            var detailsFactory = new FrameworkElementFactory(typeof(TextBlock));
            detailsFactory.SetBinding(TextBlock.TextProperty, new Binding(nameof(EditableRow.Name)));
            var grid = new ZenDataGrid
            {
                Width = 620,
                Height = 280,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                HeadersVisibility = DataGridHeadersVisibility.All,
                RowHeaderWidth = 36,
                RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected,
                SelectionMode = DataGridSelectionMode.Extended,
                FlowDirection = FlowDirection.RightToLeft,
                ItemsSource = rows,
                RowDetailsTemplate = new DataTemplate { VisualTree = detailsFactory },
                FrozenColumnCount = 1
            };
            grid.Columns.Add(nameColumn);
            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "编号",
                Binding = new Binding(nameof(EditableRow.Id))
            });
            return grid;
        }

        internal static Window CreateTestWindow(UIElement content, double width, double height)
        {
            return new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = width,
                Height = height,
                Content = content
            };
        }

        internal static Calendar GetDatePickerCalendar(ZenDatePicker datePicker)
        {
            var popup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;
            return popup?.Child as Calendar;
        }

        internal static void AssertScrollBarMetrics(
            ScrollBar scrollBar,
            double expectedTrackThickness,
            double expectedThumbMinLength,
            Thickness expectedThumbMargin,
            CornerRadius expectedCornerRadius)
        {
            scrollBar.ApplyTemplate();
            var track = scrollBar.Template.FindName("PART_Track", scrollBar) as Track;
            var trackBackground = scrollBar.Template.FindName("TrackBackground", scrollBar) as Border;
            Assert.IsNotNull(track);
            Assert.IsNotNull(trackBackground);
            Assert.IsNotNull(track.Thumb);
            track.Thumb.ApplyTemplate();
            var thumbShape = track.Thumb.Template.FindName("ThumbShape", track.Thumb) as Border;
            Assert.IsNotNull(thumbShape);

            if (scrollBar.Orientation == Orientation.Vertical)
            {
                Assert.AreEqual(expectedTrackThickness, trackBackground.Width);
                Assert.AreEqual(expectedThumbMinLength, track.Thumb.MinHeight);
            }
            else
            {
                Assert.AreEqual(expectedTrackThickness, trackBackground.Height);
                Assert.AreEqual(expectedThumbMinLength, track.Thumb.MinWidth);
            }

            Assert.AreEqual(expectedCornerRadius, trackBackground.CornerRadius);
            Assert.AreEqual(expectedThumbMargin, thumbShape.Margin);
            Assert.AreEqual(expectedCornerRadius, thumbShape.CornerRadius);
        }

        internal static FrameworkElement FindVisualDescendant(DependencyObject parent, string typeName)
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is FrameworkElement element && child.GetType().Name == typeName)
                {
                    return element;
                }

                var descendant = FindVisualDescendant(child, typeName);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        internal static T FindVisualDescendant<T>(DependencyObject parent)
            where T : DependencyObject
        {
            return FindVisualDescendants<T>(parent).FirstOrDefault();
        }

        internal static IEnumerable<T> FindVisualDescendants<T>(DependencyObject parent)
            where T : DependencyObject
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (var descendant in FindVisualDescendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        internal sealed class TestZenButton : ZenButton
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        internal sealed class TestZenSwitch : ZenSwitch
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        internal sealed class TestZenTextBox : ZenTextBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        internal sealed class TestZenCheckBox : ZenCheckBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenRadioButton : ZenRadioButton
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenComboBox : ZenComboBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenListBox : ZenListBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenDatePicker : ZenDatePicker
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenDateTimePicker : ZenDateTimePicker
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenDataGrid : ZenDataGrid
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenPasswordBox : ZenPasswordBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenSlider : ZenSlider
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenProgressBar : ZenProgressBar
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenLoading : ZenLoading
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenAlert : ZenAlert
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        internal sealed class TestZenExpander : ZenExpander
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        internal sealed class InvalidModel : IDataErrorInfo
        {
            public string Value { get; set; }

            public string Error => "输入无效";

            public string this[string columnName] => columnName == nameof(Value) ? Error : null;
        }

        internal sealed class EditableRow
        {
            public EditableRow(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; }

            public string Name { get; set; }
        }

        internal sealed class DisplayItem
        {
            public DisplayItem(string displayName)
            {
                DisplayName = displayName;
            }

            public string DisplayName { get; }
        }
    }
}
