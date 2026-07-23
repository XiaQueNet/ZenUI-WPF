using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观的滑块。
    /// </summary>
    public class ZenSlider : Slider
    {
        static ZenSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenSlider),
                new FrameworkPropertyMetadata(typeof(ZenSlider)));
        }
    }
}
