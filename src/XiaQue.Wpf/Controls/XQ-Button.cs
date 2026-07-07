using System;
using System.Windows;
using System.Windows.Controls;

namespace XiaQue.Wpf.Controls
{
    public class XQ_Button : Button
    {

        private static readonly Type SelfType = typeof(XQ_Button);

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



    }
}
