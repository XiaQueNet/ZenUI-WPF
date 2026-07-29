using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持自定义圆角和空状态内容的数据网格控件。
    /// </summary>
    public class ZenDataGrid : DataGrid
    {
        static ZenDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(typeof(ZenDataGrid)));
        }

        /// <summary>
        /// 获取或设置数据网格的圆角。
        /// </summary>
        [Bindable(true)]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CornerRadius"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// 获取或设置数据网格没有数据时显示的内容。
        /// </summary>
        [Bindable(true)]
        public object EmptyContent
        {
            get { return GetValue(EmptyContentProperty); }
            set { SetValue(EmptyContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="EmptyContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(
                nameof(EmptyContent),
                typeof(object),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata("暂无数据"));

        /// <summary>
        /// 获取或设置列标题区域的背景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush ColumnHeaderBackground
        {
            get { return (Brush)GetValue(ColumnHeaderBackgroundProperty); }
            set { SetValue(ColumnHeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ColumnHeaderBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderBackgroundProperty =
            DependencyProperty.Register(
                nameof(ColumnHeaderBackground),
                typeof(Brush),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置列标题内容的前景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush ColumnHeaderForeground
        {
            get { return (Brush)GetValue(ColumnHeaderForegroundProperty); }
            set { SetValue(ColumnHeaderForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ColumnHeaderForeground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderForegroundProperty =
            DependencyProperty.Register(
                nameof(ColumnHeaderForeground),
                typeof(Brush),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置一个值，该值指示选中行是否使用主题选择高亮。
        /// </summary>
        [Bindable(true)]
        public bool IsRowSelectionHighlightEnabled
        {
            get { return (bool)GetValue(IsRowSelectionHighlightEnabledProperty); }
            set { SetValue(IsRowSelectionHighlightEnabledProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsRowSelectionHighlightEnabled"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsRowSelectionHighlightEnabledProperty =
            DependencyProperty.RegisterAttached(
                nameof(IsRowSelectionHighlightEnabled),
                typeof(bool),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// 获取一个值，该值指示指定元素是否使用主题选择高亮。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>
        /// 如果指定元素使用主题选择高亮，则为 <see langword="true"/>；否则为
        /// <see langword="false"/>。
        /// </returns>
        public static bool GetIsRowSelectionHighlightEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsRowSelectionHighlightEnabledProperty);
        }

        /// <summary>
        /// 设置指定元素是否使用主题选择高亮。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">
        /// 如果使用主题选择高亮，则为 <see langword="true"/>；否则为
        /// <see langword="false"/>。
        /// </param>
        public static void SetIsRowSelectionHighlightEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsRowSelectionHighlightEnabledProperty, value);
        }

        /// <summary>
        /// 获取或设置一个值，该值指示当前单元格是否显示键盘焦点边框。
        /// </summary>
        [Bindable(true)]
        public bool IsCellFocusVisualEnabled
        {
            get { return (bool)GetValue(IsCellFocusVisualEnabledProperty); }
            set { SetValue(IsCellFocusVisualEnabledProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsCellFocusVisualEnabled"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsCellFocusVisualEnabledProperty =
            DependencyProperty.RegisterAttached(
                nameof(IsCellFocusVisualEnabled),
                typeof(bool),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// 获取一个值，该值指示指定元素是否显示单元格键盘焦点边框。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>
        /// 如果显示单元格键盘焦点边框，则为 <see langword="true"/>；否则为
        /// <see langword="false"/>。
        /// </returns>
        public static bool GetIsCellFocusVisualEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsCellFocusVisualEnabledProperty);
        }

        /// <summary>
        /// 设置指定元素是否显示单元格键盘焦点边框。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">
        /// 如果显示单元格键盘焦点边框，则为 <see langword="true"/>；否则为
        /// <see langword="false"/>。
        /// </param>
        public static void SetIsCellFocusVisualEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsCellFocusVisualEnabledProperty, value);
        }

        /// <summary>
        /// 获取或设置当前单元格焦点视觉的边框宽度。
        /// </summary>
        [Bindable(true)]
        public Thickness CellFocusVisualBorderThickness
        {
            get { return (Thickness)GetValue(CellFocusVisualBorderThicknessProperty); }
            set { SetValue(CellFocusVisualBorderThicknessProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CellFocusVisualBorderThickness"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CellFocusVisualBorderThicknessProperty =
            DependencyProperty.RegisterAttached(
                nameof(CellFocusVisualBorderThickness),
                typeof(Thickness),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(
                    new Thickness(1),
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// 获取指定元素的单元格焦点视觉边框宽度。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素的单元格焦点视觉边框宽度。</returns>
        public static Thickness GetCellFocusVisualBorderThickness(DependencyObject element)
        {
            return (Thickness)element.GetValue(CellFocusVisualBorderThicknessProperty);
        }

        /// <summary>
        /// 设置指定元素的单元格焦点视觉边框宽度。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的边框宽度。</param>
        public static void SetCellFocusVisualBorderThickness(DependencyObject element, Thickness value)
        {
            element.SetValue(CellFocusVisualBorderThicknessProperty, value);
        }

        /// <summary>
        /// 获取或设置单元格验证错误的边框宽度。
        /// </summary>
        [Bindable(true)]
        public Thickness CellValidationBorderThickness
        {
            get { return (Thickness)GetValue(CellValidationBorderThicknessProperty); }
            set { SetValue(CellValidationBorderThicknessProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CellValidationBorderThickness"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CellValidationBorderThicknessProperty =
            DependencyProperty.RegisterAttached(
                nameof(CellValidationBorderThickness),
                typeof(Thickness),
                typeof(ZenDataGrid),
                new FrameworkPropertyMetadata(
                    new Thickness(2),
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// 获取指定元素的单元格验证错误边框宽度。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素的单元格验证错误边框宽度。</returns>
        public static Thickness GetCellValidationBorderThickness(DependencyObject element)
        {
            return (Thickness)element.GetValue(CellValidationBorderThicknessProperty);
        }

        /// <summary>
        /// 设置指定元素的单元格验证错误边框宽度。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的边框宽度。</param>
        public static void SetCellValidationBorderThickness(DependencyObject element, Thickness value)
        {
            element.SetValue(CellValidationBorderThicknessProperty, value);
        }
    }
}
