using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示一组以按钮形式呈现、且只允许选择一个选项的控件。
    /// </summary>
    public class ZenRadioGroup : Selector
    {
        private static readonly System.Type SelfType = typeof(ZenRadioGroup);

        static ZenRadioGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
            FocusableProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// 初始化 <see cref="ZenRadioGroup"/> 类的新实例。
        /// </summary>
        public ZenRadioGroup()
        {
            ItemContainerGenerator.StatusChanged += OnContainerGeneratorStatusChanged;
        }

        /// <summary>
        /// 获取或设置选项的排列方向，可设为水平或垂直。
        /// </summary>
        [Bindable(true)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Orientation"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                SelfType,
                new FrameworkPropertyMetadata(
                    Orientation.Horizontal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 获取或设置相邻选项之间的间距。该值必须为大于或等于零的有限值。
        /// </summary>
        [Bindable(true)]
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Spacing"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(
                nameof(Spacing),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(
                    10d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidSpacing);

        /// <summary>
        /// 获取或设置一个值，该值指示是否沿排列方向为所有可见选项分配相同尺寸。
        /// </summary>
        [Bindable(true)]
        public bool IsItemWidthUniform
        {
            get { return (bool)GetValue(IsItemWidthUniformProperty); }
            set { SetValue(IsItemWidthUniformProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsItemWidthUniform"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsItemWidthUniformProperty =
            DependencyProperty.Register(
                nameof(IsItemWidthUniform),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 获取或设置选项的显示模式。
        /// </summary>
        [Bindable(true)]
        public RadioGroupDisplayMode DisplayMode
        {
            get { return (RadioGroupDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DisplayMode"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
                nameof(DisplayMode),
                typeof(RadioGroupDisplayMode),
                SelfType,
                new FrameworkPropertyMetadata(RadioGroupDisplayMode.Button));

        /// <summary>
        /// 获取或设置按钮选项的视觉样式，可选描边样式或填充样式。
        /// </summary>
        [Bindable(true)]
        public RadioGroupAppearance Appearance
        {
            get { return (RadioGroupAppearance)GetValue(AppearanceProperty); }
            set { SetValue(AppearanceProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Appearance"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty AppearanceProperty =
            DependencyProperty.Register(
                nameof(Appearance),
                typeof(RadioGroupAppearance),
                SelfType,
                new FrameworkPropertyMetadata(RadioGroupAppearance.Outlined));

        /// <summary>
        /// 获取或设置选中项的边框或背景所使用的强调色画刷，具体用法取决于
        /// <see cref="Appearance"/>。
        /// </summary>
        [Bindable(true)]
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="AccentBrush"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register(
                nameof(AccentBrush),
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ZenRadioItem;
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ZenRadioItem();
        }

        /// <inheritdoc />
        protected override void PrepareContainerForItemOverride(
            DependencyObject element,
            object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            UpdateContainerTabStops();
        }

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdateContainerTabStops();
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateContainerTabStops();
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled || Items.Count == 0)
            {
                return;
            }

            var direction = 0;
            switch (e.Key)
            {
                case Key.Left:
                    direction = FlowDirection == FlowDirection.RightToLeft ? 1 : -1;
                    break;
                case Key.Up:
                    direction = -1;
                    break;
                case Key.Right:
                    direction = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
                    break;
                case Key.Down:
                    direction = 1;
                    break;
                case Key.Home:
                    SelectBoundaryItem(true);
                    e.Handled = true;
                    return;
                case Key.End:
                    SelectBoundaryItem(false);
                    e.Handled = true;
                    return;
                case Key.Space:
                case Key.Enter:
                    var focusedContainer = ContainerFromElement(
                        this,
                        e.OriginalSource as DependencyObject) as ZenRadioItem;
                    if (focusedContainer != null)
                    {
                        SelectContainer(focusedContainer, false);
                        e.Handled = true;
                    }
                    return;
            }

            if (direction != 0)
            {
                MoveSelection(direction);
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenRadioGroupAutomationPeer(this);
        }

        internal void SelectContainer(ZenRadioItem container, bool focus)
        {
            var index = ItemContainerGenerator.IndexFromContainer(container);
            if (index < 0 || !container.IsEnabled)
            {
                return;
            }

            SelectedIndex = index;
            if (focus)
            {
                container.Focus();
            }
        }

        private void MoveSelection(int direction)
        {
            var startIndex = SelectedIndex;
            if (startIndex < 0)
            {
                var focusedContainer = ContainerFromElement(
                    this,
                    Keyboard.FocusedElement as DependencyObject) as ZenRadioItem;
                startIndex = focusedContainer == null
                    ? (direction > 0 ? -1 : 0)
                    : ItemContainerGenerator.IndexFromContainer(focusedContainer);
            }

            for (var offset = 1; offset <= Items.Count; offset++)
            {
                var index = (startIndex + (direction * offset)) % Items.Count;
                if (index < 0)
                {
                    index += Items.Count;
                }

                if (TrySelectIndex(index))
                {
                    return;
                }
            }
        }

        private void SelectBoundaryItem(bool first)
        {
            var index = first ? 0 : Items.Count - 1;
            var step = first ? 1 : -1;
            while (index >= 0 && index < Items.Count)
            {
                if (TrySelectIndex(index))
                {
                    return;
                }

                index += step;
            }
        }

        private bool TrySelectIndex(int index)
        {
            var container =
                ItemContainerGenerator.ContainerFromIndex(index) as ZenRadioItem;
            if (container == null || !container.IsEnabled)
            {
                return false;
            }

            SelectedIndex = index;
            container.Focus();
            return true;
        }

        private void OnContainerGeneratorStatusChanged(object sender, System.EventArgs e)
        {
            if (ItemContainerGenerator.Status ==
                System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                UpdateContainerTabStops();
            }
        }

        private void UpdateContainerTabStops()
        {
            var fallbackAssigned = false;
            for (var index = 0; index < Items.Count; index++)
            {
                var container =
                    ItemContainerGenerator.ContainerFromIndex(index) as ZenRadioItem;
                if (container == null)
                {
                    continue;
                }

                var isSelected = index == SelectedIndex;
                var isFallback = SelectedIndex < 0 &&
                    !fallbackAssigned &&
                    container.IsEnabled;
                container.IsTabStop = isSelected || isFallback;
                fallbackAssigned |= isFallback;
            }
        }

        private static bool IsValidSpacing(object value)
        {
            var spacing = (double)value;
            return !double.IsNaN(spacing) &&
                !double.IsInfinity(spacing) &&
                spacing >= 0d;
        }
    }
}
