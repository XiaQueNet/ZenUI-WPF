using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示 <see cref="ZenRadioGroup"/> 中的可选择项。
    /// </summary>
    public class ZenRadioItem : ContentControl
    {
        static ZenRadioItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenRadioItem),
                new FrameworkPropertyMetadata(typeof(ZenRadioItem)));
        }

        /// <summary>
        /// 获取一个值，该值指示当前选项是否被选中。
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsSelected"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(ZenRadioItem));

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled && IsEnabled)
            {
                var owner = ItemsControl.ItemsControlFromItemContainer(this) as ZenRadioGroup;
                owner?.SelectContainer(this, true);
                e.Handled = owner != null;
            }
        }
    }
}
