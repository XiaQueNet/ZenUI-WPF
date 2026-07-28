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
            DependencyProperty.Register(
                nameof(HeaderHorizontalContentAlignment),
                typeof(HorizontalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left));

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
            DependencyProperty.Register(
                nameof(HeaderVerticalContentAlignment),
                typeof(VerticalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(VerticalAlignment.Center));

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
            DependencyProperty.Register(
                nameof(CellHorizontalContentAlignment),
                typeof(HorizontalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

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
            DependencyProperty.Register(
                nameof(CellVerticalContentAlignment),
                typeof(VerticalAlignment),
                typeof(ZenDataGridTextColumn),
                new FrameworkPropertyMetadata(VerticalAlignment.Center));
    }
}
