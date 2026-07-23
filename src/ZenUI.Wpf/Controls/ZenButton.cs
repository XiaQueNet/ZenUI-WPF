using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 支持语义外观与自定义圆角的按钮。
    /// </summary>
    public class ZenButton : Button
    {
        private static readonly Type SelfType = typeof(ZenButton);

        static ZenButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置按钮圆角。
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
                SelfType,
                new FrameworkPropertyMetadata(
                    default(CornerRadius),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置按钮的语义外观。
        /// </summary>
        [Bindable(true)]
        public ButtonVariant Variant
        {
            get { return (ButtonVariant)GetValue(VariantProperty); }
            set { SetValue(VariantProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Variant"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.Register(
                nameof(Variant),
                typeof(ButtonVariant),
                SelfType,
                new FrameworkPropertyMetadata(ButtonVariant.Primary));
    }
}
