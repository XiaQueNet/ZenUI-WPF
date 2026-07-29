using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 提供 <see cref="ScrollViewer"/> 的嵌套滚动辅助功能。
    /// </summary>
    public static class ScrollViewerAssist
    {
        /// <summary>
        /// 标识 <see cref="GetIsMouseWheelChainingEnabled"/> 和
        /// <see cref="SetIsMouseWheelChainingEnabled"/> 使用的附加属性。
        /// </summary>
        public static readonly DependencyProperty IsMouseWheelChainingEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsMouseWheelChainingEnabled",
                typeof(bool),
                typeof(ScrollViewerAssist),
                new FrameworkPropertyMetadata(
                    false,
                    OnIsMouseWheelChainingEnabledChanged));

        /// <summary>
        /// 获取滚动查看器是否在没有垂直滚动范围时将鼠标滚轮事件传递给外层控件。
        /// </summary>
        /// <param name="element">要读取属性的滚动查看器。</param>
        /// <returns>启用鼠标滚轮链式滚动时为 <see langword="true"/>。</returns>
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool GetIsMouseWheelChainingEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsMouseWheelChainingEnabledProperty);
        }

        /// <summary>
        /// 设置滚动查看器是否在没有垂直滚动范围时将鼠标滚轮事件传递给外层控件。
        /// </summary>
        /// <param name="element">要设置属性的滚动查看器。</param>
        /// <param name="value">是否启用鼠标滚轮链式滚动。</param>
        public static void SetIsMouseWheelChainingEnabled(
            DependencyObject element,
            bool value)
        {
            element.SetValue(IsMouseWheelChainingEnabledProperty, value);
        }

        private static void OnIsMouseWheelChainingEnabledChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = dependencyObject as ScrollViewer;
            if (scrollViewer == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            }
            else
            {
                scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(
            object sender,
            MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (e.Handled ||
                e.Delta == 0 ||
                HasNestedScrollViewerOnEventRoute(
                    scrollViewer,
                    e.OriginalSource as DependencyObject) ||
                scrollViewer.ScrollableHeight > 0d)
            {
                return;
            }

            var parent = VisualTreeHelper.GetParent(scrollViewer) as UIElement;
            if (parent == null)
            {
                return;
            }

            e.Handled = true;
            parent.RaiseEvent(new MouseWheelEventArgs(
                e.MouseDevice,
                e.Timestamp,
                e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = parent
            });
        }

        private static bool HasNestedScrollViewerOnEventRoute(
            ScrollViewer scrollViewer,
            DependencyObject originalSource)
        {
            var current = originalSource;
            while (current != null && current != scrollViewer)
            {
                if (current is ScrollViewer)
                {
                    return true;
                }

                current = GetVisualOrLogicalParent(current);
            }

            return false;
        }

        private static DependencyObject GetVisualOrLogicalParent(
            DependencyObject element)
        {
            if (element is Visual || element is System.Windows.Media.Media3D.Visual3D)
            {
                return VisualTreeHelper.GetParent(element);
            }

            var contentElement = element as FrameworkContentElement;
            return contentElement?.Parent;
        }

    }
}
