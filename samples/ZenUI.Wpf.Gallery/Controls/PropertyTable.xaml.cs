using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Gallery.Controls
{
    public partial class PropertyTable : UserControl
    {
        private static readonly Dictionary<string, string> Descriptions =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ZenAlert.AccentBrush"] = "提示图标的强调色画刷。",
                ["ZenAlert.IconForeground"] = "提示图标的前景画刷。",
                ["ZenAlert.Variant"] = "提示条的语义类型。",
                ["ZenButton.CornerRadius"] = "按钮的圆角半径。",
                ["ZenButton.Variant"] = "按钮的语义类型。",
                ["ZenButton.Appearance"] = "按钮的视觉呈现形式。",
                ["ZenButton.HoverBackground"] = "鼠标悬浮时的背景画刷。",
                ["ZenButton.PressedBackground"] = "按下时的背景画刷。",
                ["ZenButton.HoverForeground"] = "鼠标悬浮时的前景画刷。",
                ["ZenButton.PressedForeground"] = "按下时的前景画刷。",
                ["ZenButton.HoverBorderBrush"] = "鼠标悬浮时的边框画刷。",
                ["ZenButton.PressedBorderBrush"] = "按下时的边框画刷。",
                ["ZenCalendar.DayButtonWidth"] = "日期按钮的宽度。",
                ["ZenCalendar.DayButtonHeight"] = "日期按钮的高度。",
                ["ZenCalendar.ButtonPadding"] = "月份和年份按钮的内边距。",
                ["ZenCalendar.NavigationButtonSize"] = "导航按钮的边长。",
                ["ZenCheckBox.AccentBrush"] = "选中状态的强调色画刷。",
                ["ZenCheckBox.GlyphBrush"] = "选中标记的前景画刷。",
                ["ZenCheckBox.IndicatorSize"] = "左侧选择标识的边长。",
                ["ZenComboBox.Watermark"] = "没有选中项时显示的水印。",
                ["ZenComboBox.CornerRadius"] = "下拉框的圆角半径。",
                ["ZenDataGrid.CornerRadius"] = "数据网格的圆角半径。",
                ["ZenDataGrid.EmptyContent"] = "没有数据时显示的内容。",
                ["ZenDataGrid.IsRowSelectionHighlightEnabled"] = "选中行是否使用主题选择高亮。",
                ["ZenDataGrid.IsCellFocusVisualEnabled"] = "当前单元格是否显示键盘焦点边框。",
                ["ZenDataGrid.CellFocusVisualBorderThickness"] = "当前单元格焦点视觉的边框宽度。",
                ["ZenDataGrid.CellValidationBorderThickness"] = "单元格校验错误的边框宽度。",
                ["ZenDatePicker.Watermark"] = "尚未选择日期时显示的水印。",
                ["ZenDatePicker.IsTextInputEnabled"] = "是否允许通过键盘直接输入日期。",
                ["ZenDatePicker.CornerRadius"] = "日期输入框的圆角半径。",
                ["ZenDatePicker.CalendarDayButtonWidth"] = "日历日期按钮的宽度。",
                ["ZenDatePicker.CalendarDayButtonHeight"] = "日历日期按钮的高度。",
                ["ZenDatePicker.CalendarButtonPadding"] = "日历月份和年份按钮的内边距。",
                ["ZenDatePicker.CalendarNavigationButtonSize"] = "日历导航按钮的边长。",
                ["ZenDatePicker.CalendarPopupWidth"] = "日历弹层的宽度。",
                ["ZenDatePicker.CalendarPopupHeight"] = "日历弹层的高度。",
                ["ZenDatePicker.CalendarFontSize"] = "日历弹层内容的字号。",
                ["ZenListBox.CornerRadius"] = "列表框的圆角半径。",
                ["ZenNumberBox.Value"] = "当前数值。",
                ["ZenNumberBox.Minimum"] = "允许输入的最小值。",
                ["ZenNumberBox.Maximum"] = "允许输入的最大值。",
                ["ZenNumberBox.Step"] = "单次增加或减少的步长，必须大于零。",
                ["ZenNumberBox.ButtonMode"] = "增减按钮的布局方式。",
                ["ZenNumberBox.SpinButtonWidth"] = "增减按钮的宽度。",
                ["ZenNumberBox.IsReadOnly"] = "是否禁止直接编辑文本；增减按钮仍然可用。",
                ["ZenPasswordBox.IsPasswordRevealEnabled"] = "是否显示密码明文切换按钮。",
                ["ZenPasswordBox.IsPasswordRevealed"] = "当前是否以明文显示密码。",
                ["ZenPasswordBox.Watermark"] = "密码为空时显示的水印。",
                ["ZenPasswordBox.LeadingContent"] = "显示在密码输入区域之前的内容。",
                ["ZenPasswordBox.LeadingContentTemplate"] = "前置内容的数据模板。",
                ["ZenPasswordBox.TrailingContent"] = "显示在密码输入区域和显隐按钮之间的内容。",
                ["ZenPasswordBox.TrailingContentTemplate"] = "后置内容的数据模板。",
                ["ZenPasswordBox.CornerRadius"] = "密码框的圆角半径。",
                ["ZenProgressBar.CornerRadius"] = "进度条的圆角半径。",
                ["ZenRadioButton.AccentBrush"] = "选中状态的强调色画刷。",
                ["ZenRadioButton.IndicatorSize"] = "左侧选择标识的直径。",
                ["ZenSlider.TrackThickness"] = "滑块轨道的厚度。",
                ["ZenSlider.ThumbBrush"] = "滑块 Thumb 的填充画刷。",
                ["ZenSlider.ThumbHoverBrush"] = "鼠标悬浮时 Thumb 的填充画刷。",
                ["ZenTextBox.Watermark"] = "无内容且未获得键盘焦点时显示的水印。",
                ["ZenTextBox.LeadingContent"] = "显示在文本输入区域之前的内容。",
                ["ZenTextBox.LeadingContentTemplate"] = "前置内容的数据模板。",
                ["ZenTextBox.TrailingContent"] = "显示在文本输入区域之后的内容。",
                ["ZenTextBox.TrailingContentTemplate"] = "后置内容的数据模板。",
                ["ZenTextBox.CornerRadius"] = "输入框的圆角半径。",
            };

        private static readonly Dictionary<string, string> ControlVersions =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ZenAlert"] = "0.1.0-preview.4",
                ["ZenButton"] = "0.1.0-preview.2",
                ["ZenCalendar"] = "0.1.0-preview.3",
                ["ZenCheckBox"] = "0.1.0-preview.4",
                ["ZenComboBox"] = "0.1.0-preview.1",
                ["ZenDataGrid"] = "0.1.0-preview.1",
                ["ZenDatePicker"] = "0.1.0-preview.2",
                ["ZenListBox"] = "0.1.0-preview.3",
                ["ZenNumberBox"] = "0.1.0-preview.2",
                ["ZenPasswordBox"] = "0.1.0-preview.2",
                ["ZenProgressBar"] = "0.1.0-preview.1",
                ["ZenRadioButton"] = "0.1.0-preview.4",
                ["ZenSlider"] = "0.1.0-preview.3",
                ["ZenTextBox"] = "0.1.0-preview.2",
            };

        private static readonly Dictionary<string, string> PropertyVersionOverrides =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ZenAlert.Variant"] = "0.1.0-preview.1",
                ["ZenButton.CornerRadius"] = "0.1.0-preview.1",
                ["ZenButton.Variant"] = "0.1.0-preview.1",
                ["ZenDataGrid.IsRowSelectionHighlightEnabled"] = "0.1.0-preview.3",
                ["ZenDataGrid.IsCellFocusVisualEnabled"] = "0.1.0-preview.3",
                ["ZenDataGrid.CellFocusVisualBorderThickness"] = "0.1.0-preview.3",
                ["ZenDataGrid.CellValidationBorderThickness"] = "0.1.0-preview.3",
                ["ZenDatePicker.CalendarDayButtonWidth"] = "0.1.0-preview.3",
                ["ZenDatePicker.CalendarDayButtonHeight"] = "0.1.0-preview.3",
                ["ZenDatePicker.CalendarButtonPadding"] = "0.1.0-preview.3",
                ["ZenDatePicker.CalendarNavigationButtonSize"] = "0.1.0-preview.3",
                ["ZenDatePicker.CalendarPopupWidth"] = "0.1.0-preview.4",
                ["ZenDatePicker.CalendarPopupHeight"] = "0.1.0-preview.4",
                ["ZenDatePicker.CalendarFontSize"] = "0.1.0-preview.4",
                ["ZenPasswordBox.Watermark"] = "0.1.0-preview.1",
                ["ZenPasswordBox.CornerRadius"] = "0.1.0-preview.1",
                ["ZenSlider.ThumbBrush"] = "0.1.0-preview.4",
                ["ZenSlider.ThumbHoverBrush"] = "0.1.0-preview.4",
                ["ZenTextBox.Watermark"] = "0.1.0-preview.1",
                ["ZenTextBox.CornerRadius"] = "0.1.0-preview.1",
            };

        public static readonly DependencyProperty ControlTypeProperty =
            DependencyProperty.Register(
                nameof(ControlType),
                typeof(Type),
                typeof(PropertyTable),
                new PropertyMetadata(null, OnControlTypeChanged));

        public PropertyTable()
        {
            InitializeComponent();
            Rows = Array.Empty<PropertyRow>();
        }

        public Type ControlType
        {
            get { return (Type)GetValue(ControlTypeProperty); }
            set { SetValue(ControlTypeProperty, value); }
        }

        public PropertyRow[] Rows { get; private set; }

        private static void OnControlTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyTable)dependencyObject).RefreshRows();
        }

        private void RefreshRows()
        {
            Rows = ControlType == null
                ? Array.Empty<PropertyRow>()
                : CreateRows(ControlType);

            if (PropertiesGrid == null || EmptyState == null)
            {
                return;
            }

            PropertiesGrid.ItemsSource = Rows;
            PropertiesGrid.Visibility = Rows.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
            EmptyState.Visibility = Rows.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private static PropertyRow[] CreateRows(Type controlType)
        {
            return controlType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(IsUserFacingDependencyProperty)
                .OrderBy(property => property.MetadataToken)
                .Select(property => CreateRow(controlType, property))
                .ToArray();
        }

        private static bool IsUserFacingDependencyProperty(PropertyInfo property)
        {
            var editorBrowsable = (EditorBrowsableAttribute)Attribute.GetCustomAttribute(
                property,
                typeof(EditorBrowsableAttribute));

            if (editorBrowsable?.State == EditorBrowsableState.Never)
            {
                return false;
            }

            var field = property.DeclaringType?.GetField(
                property.Name + "Property",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return field?.FieldType == typeof(DependencyProperty);
        }

        private static PropertyRow CreateRow(Type controlType, PropertyInfo property)
        {
            var field = controlType.GetField(
                property.Name + "Property",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var dependencyProperty = (DependencyProperty)field.GetValue(null);
            var key = controlType.Name + "." + property.Name;

            return new PropertyRow
            {
                Name = property.Name,
                Description = Descriptions.TryGetValue(key, out var description)
                    ? description
                    : "ZenUI 为此控件新增的公开属性。",
                TypeName = FormatType(property.PropertyType),
                DefaultValue = FormatDefaultValue(
                    dependencyProperty.GetMetadata(controlType).DefaultValue,
                    property.PropertyType),
                IntroducedVersion = GetIntroducedVersion(controlType, key),
            };
        }

        private static string GetIntroducedVersion(Type controlType, string propertyKey)
        {
            if (PropertyVersionOverrides.TryGetValue(propertyKey, out var propertyVersion))
            {
                return propertyVersion;
            }

            return ControlVersions.TryGetValue(controlType.Name, out var controlVersion)
                ? controlVersion
                : "Unreleased";
        }

        private static string FormatType(Type type)
        {
            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(decimal))
            {
                return "decimal";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(object))
            {
                return "object";
            }

            var nullableType = Nullable.GetUnderlyingType(type);
            return nullableType == null ? type.Name : FormatType(nullableType) + "?";
        }

        private static string FormatDefaultValue(object value, Type propertyType)
        {
            if (value == null)
            {
                return typeof(Brush).IsAssignableFrom(propertyType)
                    ? "未设置（使用主题）"
                    : "—";
            }

            if (value is string text)
            {
                return string.IsNullOrEmpty(text) ? "\"\"" : "\"" + text + "\"";
            }

            if (value is bool boolean)
            {
                return boolean ? "true" : "false";
            }

            if (value is double number && double.IsNaN(number))
            {
                return "Auto";
            }

            if (value is CornerRadius cornerRadius)
            {
                return cornerRadius.TopLeft == cornerRadius.TopRight
                    && cornerRadius.TopLeft == cornerRadius.BottomRight
                    && cornerRadius.TopLeft == cornerRadius.BottomLeft
                    ? FormatNumber(cornerRadius.TopLeft)
                    : string.Join(
                        ",",
                        FormatNumber(cornerRadius.TopLeft),
                        FormatNumber(cornerRadius.TopRight),
                        FormatNumber(cornerRadius.BottomRight),
                        FormatNumber(cornerRadius.BottomLeft));
            }

            if (value is Thickness thickness)
            {
                return thickness.Left == thickness.Top
                    && thickness.Left == thickness.Right
                    && thickness.Left == thickness.Bottom
                    ? FormatNumber(thickness.Left)
                    : string.Join(
                        ",",
                        FormatNumber(thickness.Left),
                        FormatNumber(thickness.Top),
                        FormatNumber(thickness.Right),
                        FormatNumber(thickness.Bottom));
            }

            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }

        private static string FormatNumber(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public sealed class PropertyRow
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string TypeName { get; set; }

        public string DefaultValue { get; set; }

        public string IntroducedVersion { get; set; }
    }
}
