using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观的单选按钮。
    /// </summary>
    public class ZenRadioButton : RadioButton
    {
        private static readonly System.Type SelfType = typeof(ZenRadioButton);

        static ZenRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        [Bindable(true)]
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        public static readonly DependencyProperty AccentBrushProperty =
            RegisterBrush(nameof(AccentBrush));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBackground
        {
            get { return (Brush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ThemeBackgroundProperty =
            RegisterBrush(nameof(ThemeBackground));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBorderBrush
        {
            get { return (Brush)GetValue(ThemeBorderBrushProperty); }
            set { SetValue(ThemeBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty ThemeBorderBrushProperty =
            RegisterBrush(nameof(ThemeBorderBrush));

        private static DependencyProperty RegisterBrush(string name)
        {
            return DependencyProperty.Register(
                name,
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));
        }
    }
}
