using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的文本输入控件。
    /// </summary>
    public class ZenTextBox : TextBox
    {
        private static readonly Type SelfType = typeof(ZenTextBox);

        static ZenTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置输入框没有内容且未获得键盘焦点时显示的水印。
        /// </summary>
        [Bindable(true)]
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Watermark"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(
                nameof(Watermark),
                typeof(string),
                SelfType,
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置显示在文本输入区域之前的内容。
        /// </summary>
        [Bindable(true)]
        public object LeadingContent
        {
            get { return GetValue(LeadingContentProperty); }
            set { SetValue(LeadingContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="LeadingContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LeadingContentProperty =
            DependencyProperty.Register(
                nameof(LeadingContent),
                typeof(object),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示 <see cref="LeadingContent"/> 的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate LeadingContentTemplate
        {
            get { return (DataTemplate)GetValue(LeadingContentTemplateProperty); }
            set { SetValue(LeadingContentTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="LeadingContentTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LeadingContentTemplateProperty =
            DependencyProperty.Register(
                nameof(LeadingContentTemplate),
                typeof(DataTemplate),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置显示在文本输入区域之后的内容。
        /// </summary>
        [Bindable(true)]
        public object TrailingContent
        {
            get { return GetValue(TrailingContentProperty); }
            set { SetValue(TrailingContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TrailingContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TrailingContentProperty =
            DependencyProperty.Register(
                nameof(TrailingContent),
                typeof(object),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示 <see cref="TrailingContent"/> 的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate TrailingContentTemplate
        {
            get { return (DataTemplate)GetValue(TrailingContentTemplateProperty); }
            set { SetValue(TrailingContentTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TrailingContentTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TrailingContentTemplateProperty =
            DependencyProperty.Register(
                nameof(TrailingContentTemplate),
                typeof(DataTemplate),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置输入框的圆角。
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
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
