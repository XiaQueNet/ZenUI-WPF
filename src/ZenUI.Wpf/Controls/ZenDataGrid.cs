using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
        /// 获取或设置选中行是否使用主题选择高亮。
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

        public static bool GetIsRowSelectionHighlightEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsRowSelectionHighlightEnabledProperty);
        }

        public static void SetIsRowSelectionHighlightEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsRowSelectionHighlightEnabledProperty, value);
        }

        /// <summary>
        /// 获取或设置当前单元格是否显示键盘焦点边框。
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

        public static bool GetIsCellFocusVisualEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsCellFocusVisualEnabledProperty);
        }

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

        public static Thickness GetCellFocusVisualBorderThickness(DependencyObject element)
        {
            return (Thickness)element.GetValue(CellFocusVisualBorderThicknessProperty);
        }

        public static void SetCellFocusVisualBorderThickness(DependencyObject element, Thickness value)
        {
            element.SetValue(CellFocusVisualBorderThicknessProperty, value);
        }

        /// <summary>
        /// 获取或设置单元格校验错误的边框宽度。
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

        public static Thickness GetCellValidationBorderThickness(DependencyObject element)
        {
            return (Thickness)element.GetValue(CellValidationBorderThicknessProperty);
        }

        public static void SetCellValidationBorderThickness(DependencyObject element, Thickness value)
        {
            element.SetValue(CellValidationBorderThicknessProperty, value);
        }
    }
}
