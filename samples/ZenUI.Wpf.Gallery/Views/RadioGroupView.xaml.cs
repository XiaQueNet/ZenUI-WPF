using System.Collections.Generic;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class RadioGroupView : UserControl
    {
        public RadioGroupView()
        {
            InitializeComponent();
        }

        public IReadOnlyList<string> Categories { get; } = new[]
        {
            "全部",
            "用品百货",
            "测试",
            "运动品牌",
            "鲜肉",
            "生鲜"
        };

        public IReadOnlyList<string> ShortOptions { get; } = new[]
        {
            "选项 A",
            "选项 B",
            "选项 C"
        };

        public IReadOnlyList<string> PlanOptions { get; } = new[]
        {
            "标准版",
            "专业版",
            "企业版"
        };
    }
}
