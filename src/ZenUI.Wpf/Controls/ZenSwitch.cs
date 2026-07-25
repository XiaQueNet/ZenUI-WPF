using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示具有开关外观的双态选择控件。
    /// </summary>
    public class ZenSwitch : ToggleButton
    {
        private static readonly Type SelfType = typeof(ZenSwitch);

        static ZenSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置由开关状态提供的默认轨道背景画刷。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBackground
        {
            get { return (Brush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ThemeBackgroundProperty =
            DependencyProperty.Register(
                nameof(ThemeBackground),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        private static readonly DependencyPropertyKey CapsuleCornerRadiusPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(CapsuleCornerRadius),
                typeof(CornerRadius),
                SelfType,
                new FrameworkPropertyMetadata(default(CornerRadius)));

        /// <summary>
        /// 标识 <see cref="CapsuleCornerRadius"/> 只读依赖属性。
        /// </summary>
        public static readonly DependencyProperty CapsuleCornerRadiusProperty =
            CapsuleCornerRadiusPropertyKey.DependencyProperty;

        /// <summary>
        /// 获取与开关实际高度相匹配的胶囊圆角。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CornerRadius CapsuleCornerRadius
        {
            get { return (CornerRadius)GetValue(CapsuleCornerRadiusProperty); }
        }

        /// <inheritdoc/>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            var radius = Math.Max(0d, sizeInfo.NewSize.Height / 2d);
            SetValue(
                CapsuleCornerRadiusPropertyKey,
                new CornerRadius(radius));
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenSwitchAutomationPeer(this);
        }

        private sealed class ZenSwitchAutomationPeer : ToggleButtonAutomationPeer
        {
            public ZenSwitchAutomationPeer(ZenSwitch owner)
                : base(owner)
            {
            }

            protected override string GetClassNameCore()
            {
                return nameof(ZenSwitch);
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.CheckBox;
            }
        }
    }
}
