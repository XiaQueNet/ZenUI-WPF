using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持直接配置列标题内容对齐方式的文本列。
    /// </summary>
    public class ZenDataGridTextColumn : DataGridTextColumn
    {
        /// <summary>
        /// 获取或设置列标题内容的水平对齐方式。
        /// </summary>
        [Bindable(true)]
        public HorizontalAlignment HeaderHorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderHorizontalContentAlignmentProperty); }
            set { SetValue(HeaderHorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HeaderHorizontalContentAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HeaderHorizontalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                nameof(HeaderHorizontalContentAlignment),
                typeof(HorizontalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// 获取指定元素中列标题内容的水平对齐方式。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素中列标题内容的水平对齐方式。</returns>
        public static HorizontalAlignment GetHeaderHorizontalContentAlignment(DependencyObject element)
        {
            return (HorizontalAlignment)element.GetValue(HeaderHorizontalContentAlignmentProperty);
        }

        /// <summary>
        /// 设置指定元素中列标题内容的水平对齐方式。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的水平对齐方式。</param>
        public static void SetHeaderHorizontalContentAlignment(DependencyObject element, HorizontalAlignment value)
        {
            element.SetValue(HeaderHorizontalContentAlignmentProperty, value);
        }

        /// <summary>
        /// 获取或设置列标题内容的垂直对齐方式。
        /// </summary>
        [Bindable(true)]
        public VerticalAlignment HeaderVerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(HeaderVerticalContentAlignmentProperty); }
            set { SetValue(HeaderVerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HeaderVerticalContentAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HeaderVerticalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                nameof(HeaderVerticalContentAlignment),
                typeof(VerticalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// 获取指定元素中列标题内容的垂直对齐方式。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素中列标题内容的垂直对齐方式。</returns>
        public static VerticalAlignment GetHeaderVerticalContentAlignment(DependencyObject element)
        {
            return (VerticalAlignment)element.GetValue(HeaderVerticalContentAlignmentProperty);
        }

        /// <summary>
        /// 设置指定元素中列标题内容的垂直对齐方式。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的垂直对齐方式。</param>
        public static void SetHeaderVerticalContentAlignment(DependencyObject element, VerticalAlignment value)
        {
            element.SetValue(HeaderVerticalContentAlignmentProperty, value);
        }

        /// <summary>
        /// 获取或设置单元格内容的水平对齐方式。
        /// </summary>
        [Bindable(true)]
        public HorizontalAlignment CellHorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(CellHorizontalContentAlignmentProperty); }
            set { SetValue(CellHorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CellHorizontalContentAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CellHorizontalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                nameof(CellHorizontalContentAlignment),
                typeof(HorizontalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// 获取指定元素中单元格内容的水平对齐方式。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素中单元格内容的水平对齐方式。</returns>
        public static HorizontalAlignment GetCellHorizontalContentAlignment(DependencyObject element)
        {
            return (HorizontalAlignment)element.GetValue(CellHorizontalContentAlignmentProperty);
        }

        /// <summary>
        /// 设置指定元素中单元格内容的水平对齐方式。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的水平对齐方式。</param>
        public static void SetCellHorizontalContentAlignment(DependencyObject element, HorizontalAlignment value)
        {
            element.SetValue(CellHorizontalContentAlignmentProperty, value);
        }

        /// <summary>
        /// 获取或设置单元格内容的垂直对齐方式。
        /// </summary>
        [Bindable(true)]
        public VerticalAlignment CellVerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(CellVerticalContentAlignmentProperty); }
            set { SetValue(CellVerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CellVerticalContentAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CellVerticalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                nameof(CellVerticalContentAlignment),
                typeof(VerticalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// 获取指定元素中单元格内容的垂直对齐方式。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素中单元格内容的垂直对齐方式。</returns>
        public static VerticalAlignment GetCellVerticalContentAlignment(DependencyObject element)
        {
            return (VerticalAlignment)element.GetValue(CellVerticalContentAlignmentProperty);
        }

        /// <summary>
        /// 设置指定元素中单元格内容的垂直对齐方式。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的垂直对齐方式。</param>
        public static void SetCellVerticalContentAlignment(DependencyObject element, VerticalAlignment value)
        {
            element.SetValue(CellVerticalContentAlignmentProperty, value);
        }
    }
}
