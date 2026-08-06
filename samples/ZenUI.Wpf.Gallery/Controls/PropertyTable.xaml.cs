using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace ZenUI.Wpf.Gallery.Controls
{
    public partial class PropertyTable : UserControl
    {
        private const string UnreleasedPropertyVersion = "未发布";

        private static readonly Dictionary<Assembly, Dictionary<string, string>>
            XmlDescriptionsByAssembly =
                new Dictionary<Assembly, Dictionary<string, string>>();

        private static readonly Dictionary<string, string> Descriptions =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ZenAlert.IconBackground"] = "提示图标的背景画刷。",
                ["ZenAlert.IconForeground"] = "提示图标的前景画刷。",
                ["ZenAlert.IconSize"] = "提示图标的边长。",
                ["ZenAlert.Severity"] = "提示条的严重级别。",
                ["ZenButton.CornerRadius"] = "按钮的圆角半径。",
                ["ZenButton.Variant"] = "按钮的语义类型。",
                ["ZenButton.Appearance"] = "按钮的视觉呈现形式。",
                ["ZenButton.HoverBackground"] = "鼠标悬浮时的背景画刷。",
                ["ZenButton.PressedBackground"] = "按下时的背景画刷。",
                ["ZenButton.HoverForeground"] = "鼠标悬浮时的前景画刷。",
                ["ZenButton.PressedForeground"] = "按下时的前景画刷。",
                ["ZenButton.HoverBorderBrush"] = "鼠标悬浮时的边框画刷。",
                ["ZenButton.PressedBorderBrush"] = "按下时的边框画刷。",
                ["ZenCalendar.CornerRadius"] = "日历外框的圆角半径。",
                ["ZenCheckBox.CheckedBackground"] = "选中或不确定状态下选择标识的背景画刷。",
                ["ZenCheckBox.CheckedBorderBrush"] = "选中或不确定状态下选择标识的边框画刷。",
                ["ZenCheckBox.HoverBorderBrush"] = "鼠标悬停时选择标识的边框画刷。",
                ["ZenCheckBox.CheckedGlyphBrush"] = "勾号或不确定标记的前景画刷。",
                ["ZenCheckBox.IndicatorSize"] = "左侧选择标识的边长。",
                ["ZenComboBox.Watermark"] = "没有选中项时显示的水印。",
                ["ZenComboBox.CornerRadius"] = "下拉框的圆角半径。",
                ["ZenDataGrid.CornerRadius"] = "数据网格的圆角半径。",
                ["ZenDataGrid.EmptyContent"] = "没有数据时显示的内容。",
                ["ZenDataGrid.ColumnHeaderBackground"] = "列标题区域的背景画刷。",
                ["ZenDataGrid.ColumnHeaderForeground"] = "列标题内容的前景画刷。",
                ["ZenDataGrid.IsRowSelectionHighlightEnabled"] = "选中行是否使用主题选择高亮。",
                ["ZenDataGrid.IsCellFocusVisualEnabled"] = "当前单元格是否显示键盘焦点边框。",
                ["ZenDataGrid.CellFocusVisualBorderThickness"] = "当前单元格焦点视觉的边框宽度。",
                ["ZenDataGrid.CellValidationBorderThickness"] = "单元格校验错误的边框宽度。",
                ["ZenDataGridTextColumn.HeaderHorizontalContentAlignment"] = "列标题内容的水平对齐方式。",
                ["ZenDataGridTextColumn.HeaderVerticalContentAlignment"] = "列标题内容的垂直对齐方式。",
                ["ZenDataGridTextColumn.CellHorizontalContentAlignment"] = "单元格内容的水平对齐方式。",
                ["ZenDataGridTextColumn.CellVerticalContentAlignment"] = "单元格内容的垂直对齐方式。",
                ["ZenDatePicker.Watermark"] = "尚未选择日期时显示的水印。",
                ["ZenDatePicker.IsTextInputReadOnly"] = "日期文本输入是否只读。",
                ["ZenDatePicker.CornerRadius"] = "日期输入框的圆角半径。",
                ["ZenDatePicker.CalendarPopupWidth"] = "日历弹层的宽度。",
                ["ZenDatePicker.CalendarPopupHeight"] = "日历弹层的高度。",
                ["ZenDatePicker.CalendarFontSize"] = "日历弹层内容的字号。",
                ["ZenTimePicker.SelectedTime"] = "当前选中的一天内时间。",
                ["ZenTimePicker.Minimum"] = "允许选择的最早时间。",
                ["ZenTimePicker.Maximum"] = "允许选择的最晚时间。",
                ["ZenTimePicker.MinuteIncrement"] = "分钟列表的递增步长。",
                ["ZenTimePicker.SecondIncrement"] = "秒列表的递增步长。",
                ["ZenTimePicker.IsSecondVisible"] = "是否显示和编辑秒。",
                ["ZenTimePicker.Is24HourFormat"] = "是否使用 24 小时制。",
                ["ZenDateTimePicker.Is24HourFormat"] = "是否使用 24 小时制。",
                ["ZenTimePicker.Watermark"] = "尚未选择时间时显示的水印。",
                ["ZenTimePicker.IsTextInputReadOnly"] = "时间文本输入是否只读。",
                ["ZenDateTimePicker.IsTextInputReadOnly"] = "日期时间文本输入是否只读。",
                ["ZenTimePicker.CornerRadius"] = "时间输入框的圆角半径。",
                ["ZenTimePicker.IsDropDownOpen"] = "时间选择弹层是否打开。",
                ["ZenExpander.CornerRadius"] = "折叠面板外框的圆角半径。",
                ["ZenExpander.HeaderPadding"] = "标题区域的内边距。",
                ["ZenExpander.GlyphSize"] = "展开标识的边长。",
                ["ZenListBox.CornerRadius"] = "列表框的圆角半径。",
                ["ZenLoading.IsLoading"] = "是否显示加载状态。",
                ["ZenLoading.LoadingText"] = "加载指示器下方显示的说明文字。",
                ["ZenLoading.IndicatorSize"] = "圆形加载指示器的边长。",
                ["ZenLoading.Orientation"] = "加载指示器与说明文字的排列方向。",
                ["ZenLoading.OverlayBackground"] = "加载层的背景画刷。",
                ["ZenLoading.IsContentInteractionBlocked"] = "加载时是否阻止用户与内容交互。",
                ["ZenNumberBox.Value"] = "当前数值；编辑器中的有效输入会实时更新该值。",
                ["ZenNumberBox.Minimum"] = "允许输入的最小值。",
                ["ZenNumberBox.Maximum"] = "允许输入的最大值。",
                ["ZenNumberBox.Increment"] = "单次增加或减少的步长，必须大于零。",
                ["ZenNumberBox.SpinButtonLayout"] = "增减按钮的布局方式。",
                ["ZenNumberBox.SpinButtonWidth"] = "增减按钮的宽度。",
                ["ZenNumberBox.IncreaseButtonContent"] = "增加按钮中显示的自定义内容。",
                ["ZenNumberBox.IncreaseButtonContentTemplate"] = "增加按钮内容的数据模板。",
                ["ZenNumberBox.DecreaseButtonContent"] = "减少按钮中显示的自定义内容。",
                ["ZenNumberBox.DecreaseButtonContentTemplate"] = "减少按钮内容的数据模板。",
                ["ZenNumberBox.EditorClickCommand"] = "点击数字编辑区域时执行的命令。",
                ["ZenNumberBox.EditorClickCommandParameter"] = "传递给编辑区域点击命令的参数。",
                ["ZenNumberBox.IsReadOnly"] = "是否禁止直接编辑文本；增减按钮仍然可用。",
                ["ZenPasswordBox.IsPasswordRevealButtonEnabled"] = "是否显示密码明文切换按钮。",
                ["ZenPasswordBox.IsPasswordRevealed"] = "当前是否以明文显示密码。",
                ["ZenPasswordBox.Watermark"] = "密码为空时显示的水印。",
                ["ZenPasswordBox.LeadingContent"] = "显示在密码输入区域之前的内容。",
                ["ZenPasswordBox.LeadingContentTemplate"] = "前置内容的数据模板。",
                ["ZenPasswordBox.TrailingContent"] = "显示在密码输入区域和显隐按钮之间的内容。",
                ["ZenPasswordBox.TrailingContentTemplate"] = "后置内容的数据模板。",
                ["ZenPasswordBox.CornerRadius"] = "密码框的圆角半径。",
                ["ZenProgressBar.CornerRadius"] = "进度条的圆角半径。",
                ["ZenRadioButton.HoverBorderBrush"] = "鼠标悬停时选择指示器的边框画刷。",
                ["ZenRadioButton.CheckedBorderBrush"] = "选中时选择指示器的边框画刷。",
                ["ZenRadioButton.CheckedGlyphBrush"] = "选中圆点的画刷。",
                ["ZenRadioButton.IndicatorSize"] = "左侧选择标识的直径。",
                ["ZenRadioGroup.Orientation"] = "选项的排列方向。",
                ["ZenRadioGroup.Spacing"] = "相邻选项之间的间距。",
                ["ZenRadioGroup.IsItemSizeUniform"] = "是否沿排列方向为选项分配相同尺寸。",
                ["ZenRadioGroup.Appearance"] = "选项的视觉外观。",
                ["ZenRadioGroup.SelectionBrush"] = "选中项使用的选择画刷。",
                ["ZenSlider.TrackThickness"] = "滑块轨道的厚度。",
                ["ZenSlider.ThumbBrush"] = "滑块 Thumb 的填充画刷。",
                ["ZenSlider.HoverThumbBrush"] = "鼠标悬浮时 Thumb 的填充画刷。",
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
                ["ZenAlert"] = "0.1.0-preview.1",
                ["ZenButton"] = "0.1.0-preview.1",
                ["ZenCalendar"] = "0.1.0-preview.3",
                ["ZenCheckBox"] = "0.1.0-preview.1",
                ["ZenComboBox"] = "0.1.0-preview.1",
                ["ZenContextMenu"] = "0.1.0-preview.6",
                ["ZenDataGrid"] = "0.1.0-preview.1",
                ["ZenDataGridTextColumn"] = "0.1.0-preview.6",
                ["ZenDatePicker"] = "0.1.0-preview.2",
                ["ZenDateTimePicker"] = "0.1.0-preview.7",
                ["ZenExpander"] = "0.1.0-preview.6",
                ["ZenListBox"] = "0.1.0-preview.3",
                ["ZenLoading"] = "0.1.0-preview.7",
                ["ZenNumberBox"] = "0.1.0-preview.2",
                ["ZenPasswordBox"] = "0.1.0-preview.1",
                ["ZenPopover"] = "0.1.0-preview.6",
                ["ZenProgressBar"] = "0.1.0-preview.1",
                ["ZenRadioButton"] = "0.1.0-preview.1",
                ["ZenRadioGroup"] = "0.1.0-preview.6",
                ["ZenSlider"] = "0.1.0-preview.1",
                ["ZenSwitch"] = "0.1.0-preview.1",
                ["ZenTextBox"] = "0.1.0-preview.1",
                ["ZenTimePicker"] = "0.1.0-preview.6",
            };

        // 开发中的新增或重命名属性必须显式登记为“未发布”；仅在准备发版时统一替换为实际版本号。
        private static readonly Dictionary<string, string> PropertyVersionOverrides =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ZenAlert.IconBackground"] = "0.1.0-preview.10",
                ["ZenAlert.IconForeground"] = "0.1.0-preview.4",
                ["ZenAlert.IconSize"] = "0.1.0-preview.6",
                ["ZenAlert.Severity"] = "0.1.0-preview.5",
                ["ZenButton.Appearance"] = "0.1.0-preview.2",
                ["ZenButton.HoverBackground"] = "0.1.0-preview.2",
                ["ZenButton.HoverBorderBrush"] = "0.1.0-preview.2",
                ["ZenButton.HoverForeground"] = "0.1.0-preview.2",
                ["ZenButton.PressedBackground"] = "0.1.0-preview.2",
                ["ZenButton.PressedBorderBrush"] = "0.1.0-preview.2",
                ["ZenButton.PressedForeground"] = "0.1.0-preview.2",
                ["ZenCalendar.CornerRadius"] = "0.1.0-preview.6",
                ["ZenCheckBox.CheckedBackground"] = "0.1.0-preview.10",
                ["ZenCheckBox.CheckedBorderBrush"] = "0.1.0-preview.10",
                ["ZenCheckBox.HoverBorderBrush"] = "0.1.0-preview.10",
                ["ZenCheckBox.CheckedGlyphBrush"] = "0.1.0-preview.10",
                ["ZenCheckBox.IndicatorSize"] = "0.1.0-preview.5",
                ["ZenDataGrid.IsRowSelectionHighlightEnabled"] = "0.1.0-preview.3",
                ["ZenDataGrid.IsCellFocusVisualEnabled"] = "0.1.0-preview.3",
                ["ZenDataGrid.CellFocusVisualBorderThickness"] = "0.1.0-preview.3",
                ["ZenDataGrid.CellValidationBorderThickness"] = "0.1.0-preview.3",
                ["ZenDataGrid.ColumnHeaderBackground"] = "0.1.0-preview.6",
                ["ZenDataGrid.ColumnHeaderForeground"] = "0.1.0-preview.6",
                ["ZenDatePicker.CalendarPopupWidth"] = "0.1.0-preview.4",
                ["ZenDatePicker.CalendarPopupHeight"] = "0.1.0-preview.4",
                ["ZenDatePicker.CalendarFontSize"] = "0.1.0-preview.4",
                ["ZenDatePicker.IsTextInputReadOnly"] = "0.1.0-preview.10",
                ["ZenDateTimePicker.DropDownHeight"] = "0.1.0-preview.8",
                ["ZenDateTimePicker.DropDownWidth"] = "0.1.0-preview.8",
                ["ZenDateTimePicker.DropDownButtonIconSize"] = "0.1.0-preview.10",
                ["ZenDateTimePicker.IsTextInputReadOnly"] = "0.1.0-preview.10",
                ["ZenDateTimePicker.Is24HourFormat"] = "0.1.0-preview.10",
                ["ZenDateTimePicker.CalendarCellHeight"] = "0.1.0-preview.8",
                ["ZenDateTimePicker.CalendarCellWidth"] = "0.1.0-preview.8",
                ["ZenDateTimePicker.TimeItemHeight"] = "0.1.0-preview.8",
                ["ZenDateTimePicker.TimeItemWidth"] = "0.1.0-preview.8",
                ["ZenNumberBox.DecreaseButtonContent"] = "0.1.0-preview.7",
                ["ZenNumberBox.DecreaseButtonContentTemplate"] = "0.1.0-preview.7",
                ["ZenNumberBox.EditorClickCommand"] = "0.1.0-preview.7",
                ["ZenNumberBox.EditorClickCommandParameter"] = "0.1.0-preview.7",
                ["ZenNumberBox.IncreaseButtonContent"] = "0.1.0-preview.7",
                ["ZenNumberBox.IncreaseButtonContentTemplate"] = "0.1.0-preview.7",
                ["ZenNumberBox.Increment"] = "0.1.0-preview.10",
                ["ZenNumberBox.SpinButtonLayout"] = "0.1.0-preview.5",
                ["ZenNumberBox.SpinButtonWidth"] = "0.1.0-preview.5",
                ["ZenPasswordBox.IsPasswordRevealButtonEnabled"] = "0.1.0-preview.5",
                ["ZenPasswordBox.IsPasswordRevealed"] = "0.1.0-preview.2",
                ["ZenPasswordBox.LeadingContent"] = "0.1.0-preview.2",
                ["ZenPasswordBox.LeadingContentTemplate"] = "0.1.0-preview.2",
                ["ZenPasswordBox.TrailingContent"] = "0.1.0-preview.2",
                ["ZenPasswordBox.TrailingContentTemplate"] = "0.1.0-preview.2",
                ["ZenPopover.Anchor"] = "0.1.0-preview.7",
                ["ZenPopover.AnchorButtonStyle"] = "0.1.0-preview.7",
                ["ZenPopover.AnchorTemplate"] = "0.1.0-preview.7",
                ["ZenPopover.AnchorGap"] = "0.1.0-preview.10",
                ["ZenRadioButton.HoverBorderBrush"] = "0.1.0-preview.10",
                ["ZenRadioButton.CheckedBorderBrush"] = "0.1.0-preview.10",
                ["ZenRadioButton.CheckedGlyphBrush"] = "0.1.0-preview.10",
                ["ZenRadioButton.IndicatorSize"] = "0.1.0-preview.5",
                ["ZenRadioGroup.Appearance"] = "0.1.0-preview.10",
                ["ZenRadioGroup.IsItemSizeUniform"] = "0.1.0-preview.10",
                ["ZenRadioGroup.SelectionBrush"] = "0.1.0-preview.10",
                ["ZenSlider.ThumbBrush"] = "0.1.0-preview.4",
                ["ZenSlider.HoverThumbBrush"] = "0.1.0-preview.10",
                ["ZenSlider.TrackThickness"] = "0.1.0-preview.3",
                ["ZenSwitch.CheckedContent"] = "0.1.0-preview.8",
                ["ZenSwitch.UncheckedContent"] = "0.1.0-preview.8",
                ["ZenTextBox.LeadingContent"] = "0.1.0-preview.2",
                ["ZenTextBox.LeadingContentTemplate"] = "0.1.0-preview.2",
                ["ZenTextBox.TrailingContent"] = "0.1.0-preview.2",
                ["ZenTextBox.TrailingContentTemplate"] = "0.1.0-preview.2",
                ["ZenTimePicker.IsTextInputReadOnly"] = "0.1.0-preview.10",
                ["ZenTimePicker.Is24HourFormat"] = "0.1.0-preview.10",
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

        private void CopyPropertyName_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)
                || !(button.DataContext is PropertyRow row)
                || string.IsNullOrEmpty(row.Name))
            {
                return;
            }

            try
            {
                Clipboard.SetText(row.Name);
                button.ToolTip = "已复制：" + row.Name;
            }
            catch (ExternalException)
            {
                button.ToolTip = "复制失败，请重试";
            }
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
                Description = GetDescription(property, key),
                TypeName = FormatType(property.PropertyType),
                DefaultValue = FormatDefaultValue(
                    dependencyProperty.GetMetadata(controlType).DefaultValue,
                    property.PropertyType),
                IntroducedVersion = GetIntroducedVersion(controlType, key),
            };
        }

        private static string GetDescription(PropertyInfo property, string key)
        {
            var xmlDescription = GetXmlDescription(property);
            if (!string.IsNullOrEmpty(xmlDescription))
            {
                return xmlDescription;
            }

            return Descriptions.TryGetValue(key, out var description)
                ? description
                : "ZenUI 为此控件新增的公开属性。";
        }

        private static string GetXmlDescription(PropertyInfo property)
        {
            var declaringType = property.DeclaringType;
            if (declaringType == null)
            {
                return null;
            }

            var descriptions = GetXmlDescriptions(declaringType.Assembly);
            var memberName = "P:" + declaringType.FullName + "." + property.Name;
            return descriptions.TryGetValue(memberName, out var description)
                ? description
                : null;
        }

        private static Dictionary<string, string> GetXmlDescriptions(Assembly assembly)
        {
            lock (XmlDescriptionsByAssembly)
            {
                if (XmlDescriptionsByAssembly.TryGetValue(
                    assembly,
                    out var descriptions))
                {
                    return descriptions;
                }

                descriptions = LoadXmlDescriptions(assembly);
                XmlDescriptionsByAssembly[assembly] = descriptions;
                return descriptions;
            }
        }

        private static Dictionary<string, string> LoadXmlDescriptions(Assembly assembly)
        {
            var descriptions =
                new Dictionary<string, string>(StringComparer.Ordinal);

            try
            {
                var documentationPath =
                    Path.ChangeExtension(assembly.Location, ".xml");
                if (!File.Exists(documentationPath))
                {
                    return descriptions;
                }

                var document = XDocument.Load(documentationPath);
                foreach (var member in document.Descendants("member"))
                {
                    var memberName = (string)member.Attribute("name");
                    var summary = member.Element("summary");
                    if (string.IsNullOrEmpty(memberName) || summary == null)
                    {
                        continue;
                    }

                    var description = NormalizeDocumentationText(
                        GetDocumentationText(summary));
                    if (!string.IsNullOrEmpty(description))
                    {
                        descriptions[memberName] = description;
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (XmlException)
            {
            }

            return descriptions;
        }

        private static string GetDocumentationText(XElement element)
        {
            return string.Concat(element.Nodes().Select(GetDocumentationNodeText));
        }

        private static string GetDocumentationNodeText(XNode node)
        {
            if (node is XText text)
            {
                return text.Value;
            }

            if (!(node is XElement element))
            {
                return string.Empty;
            }

            if (element.Name.LocalName == "see")
            {
                var reference = (string)element.Attribute("cref")
                    ?? (string)element.Attribute("langword");
                return FormatDocumentationReference(reference);
            }

            if (element.Name.LocalName == "paramref")
            {
                return (string)element.Attribute("name") ?? string.Empty;
            }

            return GetDocumentationText(element);
        }

        private static string FormatDocumentationReference(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return string.Empty;
            }

            var separatorIndex = reference.IndexOf(':');
            if (separatorIndex >= 0)
            {
                reference = reference.Substring(separatorIndex + 1);
            }

            var parametersIndex = reference.IndexOf('(');
            if (parametersIndex >= 0)
            {
                reference = reference.Substring(0, parametersIndex);
            }

            var memberIndex = reference.LastIndexOf('.');
            return memberIndex >= 0
                ? reference.Substring(memberIndex + 1)
                : reference;
        }

        private static string NormalizeDocumentationText(string text)
        {
            return string.Join(
                " ",
                text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string GetIntroducedVersion(Type controlType, string propertyKey)
        {
            if (PropertyVersionOverrides.TryGetValue(propertyKey, out var propertyVersion))
            {
                return propertyVersion;
            }

            return ControlVersions.TryGetValue(controlType.Name, out var controlVersion)
                ? controlVersion
                : "待核对";
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
