using System.Windows;
using System.Windows.Input;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 提供对话框窗口的操作按钮和布局选项。
    /// </summary>
    public static class ZenDialogWindowOptions
    {
        /// <summary>获取是否允许通过标题栏拖动对话框窗口。</summary>
        public static bool GetIsDraggable(DependencyObject element) => (bool)element.GetValue(IsDraggableProperty);

        /// <summary>设置是否允许通过标题栏拖动对话框窗口。</summary>
        public static void SetIsDraggable(DependencyObject element, bool value) => element.SetValue(IsDraggableProperty, value);

        /// <summary>标识是否允许拖动对话框窗口的附加属性。</summary>
        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.RegisterAttached("IsDraggable", typeof(bool), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 获取确定按钮是否显示。
        /// </summary>
        public static bool GetShowConfirmButton(DependencyObject element)
        {
            return (bool)element.GetValue(ShowConfirmButtonProperty);
        }

        /// <summary>
        /// 设置确定按钮是否显示。
        /// </summary>
        public static void SetShowConfirmButton(DependencyObject element, bool value)
        {
            element.SetValue(ShowConfirmButtonProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="GetShowConfirmButton"/> 和 <see cref="SetShowConfirmButton"/> 使用的附加属性。
        /// </summary>
        public static readonly DependencyProperty ShowConfirmButtonProperty =
            DependencyProperty.RegisterAttached(
                "ShowConfirmButton",
                typeof(bool),
                typeof(ZenDialogWindowOptions),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 获取取消按钮是否显示。
        /// </summary>
        public static bool GetShowCancelButton(DependencyObject element)
        {
            return (bool)element.GetValue(ShowCancelButtonProperty);
        }

        /// <summary>
        /// 设置取消按钮是否显示。
        /// </summary>
        public static void SetShowCancelButton(DependencyObject element, bool value)
        {
            element.SetValue(ShowCancelButtonProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="GetShowCancelButton"/> 和 <see cref="SetShowCancelButton"/> 使用的附加属性。
        /// </summary>
        public static readonly DependencyProperty ShowCancelButtonProperty =
            DependencyProperty.RegisterAttached(
                "ShowCancelButton",
                typeof(bool),
                typeof(ZenDialogWindowOptions),
                new FrameworkPropertyMetadata(true));

        /// <summary>获取确定按钮的内容。</summary>
        public static object GetConfirmContent(DependencyObject element) => element.GetValue(ConfirmContentProperty);

        /// <summary>设置确定按钮的内容。</summary>
        public static void SetConfirmContent(DependencyObject element, object value) => element.SetValue(ConfirmContentProperty, value);

        /// <summary>标识确定按钮内容的附加属性。</summary>
        public static readonly DependencyProperty ConfirmContentProperty =
            DependencyProperty.RegisterAttached("ConfirmContent", typeof(object), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata("确定"));

        /// <summary>获取取消按钮的内容。</summary>
        public static object GetCancelContent(DependencyObject element) => element.GetValue(CancelContentProperty);

        /// <summary>设置取消按钮的内容。</summary>
        public static void SetCancelContent(DependencyObject element, object value) => element.SetValue(CancelContentProperty, value);

        /// <summary>标识取消按钮内容的附加属性。</summary>
        public static readonly DependencyProperty CancelContentProperty =
            DependencyProperty.RegisterAttached("CancelContent", typeof(object), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata("取消"));

        /// <summary>获取弹窗内容区的外边距。</summary>
        public static Thickness GetContentMargin(DependencyObject element) => (Thickness)element.GetValue(ContentMarginProperty);

        /// <summary>设置弹窗内容区的外边距。</summary>
        public static void SetContentMargin(DependencyObject element, Thickness value) => element.SetValue(ContentMarginProperty, value);

        /// <summary>标识弹窗内容区外边距的附加属性。</summary>
        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.RegisterAttached("ContentMargin", typeof(Thickness), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata(new Thickness(20, 0, 20, 20)));

        /// <summary>获取底部操作区的内边距。</summary>
        public static Thickness GetFooterPadding(DependencyObject element) => (Thickness)element.GetValue(FooterPaddingProperty);

        /// <summary>设置底部操作区的内边距。</summary>
        public static void SetFooterPadding(DependencyObject element, Thickness value) => element.SetValue(FooterPaddingProperty, value);

        /// <summary>标识底部操作区内边距的附加属性。</summary>
        public static readonly DependencyProperty FooterPaddingProperty =
            DependencyProperty.RegisterAttached("FooterPadding", typeof(Thickness), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata(new Thickness(20, 12, 20, 12)));

        /// <summary>获取弹窗视图上的确定命令。</summary>
        public static ICommand GetConfirmCommand(DependencyObject element) => (ICommand)element.GetValue(ConfirmCommandProperty);

        /// <summary>设置弹窗视图上的确定命令。</summary>
        public static void SetConfirmCommand(DependencyObject element, ICommand value) => element.SetValue(ConfirmCommandProperty, value);

        /// <summary>标识弹窗视图上确定命令的附加属性。</summary>
        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.RegisterAttached("ConfirmCommand", typeof(ICommand), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata(null));

        /// <summary>获取弹窗视图上的取消命令。</summary>
        public static ICommand GetCancelCommand(DependencyObject element) => (ICommand)element.GetValue(CancelCommandProperty);

        /// <summary>设置弹窗视图上的取消命令。</summary>
        public static void SetCancelCommand(DependencyObject element, ICommand value) => element.SetValue(CancelCommandProperty, value);

        /// <summary>标识弹窗视图上取消命令的附加属性。</summary>
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.RegisterAttached("CancelCommand", typeof(ICommand), typeof(ZenDialogWindowOptions), new FrameworkPropertyMetadata(null));
    }
}
