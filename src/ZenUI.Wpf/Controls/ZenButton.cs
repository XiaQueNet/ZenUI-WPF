using System;
using System.Windows;
using System.Windows.Controls;

using ZenUI.Wpf.Enums;

namespace ZenUI.Wpf.Controls
{
    public class ZenButton : Button
    {

        private static readonly Type SelfType = typeof(ZenButton);

        /// <summary>
        /// 圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), SelfType, new PropertyMetadata(new CornerRadius(0)));





        public ButtonStyle Type
        {
            get { return (ButtonStyle)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(ButtonStyle), SelfType, new PropertyMetadata(ButtonStyle.Primary));



    }
}
