namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 指定 <see cref="ZenRadioGroup"/> 选项的视觉外观。
    /// </summary>
    public enum RadioGroupAppearance
    {
        /// <summary>
        /// 使用传统单选按钮呈现选项。
        /// </summary>
        Radio,

        /// <summary>
        /// 使用强调色填充的按钮呈现选中项。
        /// </summary>
        Filled,

        /// <summary>
        /// 使用描边按钮呈现选项。
        /// </summary>
        Outlined,

        /// <summary>
        /// 使用无常驻背景和边框的按钮呈现选项。
        /// </summary>
        Ghost,

        /// <summary>
        /// 使用下划线指示选中项。
        /// </summary>
        Underline,

        /// <summary>
        /// 使用相连的胶囊式分段控件呈现选项。
        /// </summary>
        Segmented
    }
}
