using System.Collections.Generic;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class OverviewView : UserControl
    {
        public IReadOnlyList<string> ViewModes { get; } = new[] { "列表", "看板", "时间线" };

        public IReadOnlyList<OverviewDeliveryItem> DeliveryItems { get; } = new[]
        {
            new OverviewDeliveryItem("ZenButton", "基础交互", "林晓 · Design Systems", "100%", "已完成"),
            new OverviewDeliveryItem("ZenDataGrid", "数据展示", "陈墨 · Controls", "82%", "验证中"),
            new OverviewDeliveryItem("ZenDatePicker", "数据输入", "周予 · Controls", "68%", "开发中"),
            new OverviewDeliveryItem("ZenLoading", "状态反馈", "许言 · Experience", "45%", "设计评审"),
        };

        public OverviewView() { InitializeComponent(); }
    }

    public sealed class OverviewDeliveryItem
    {
        public OverviewDeliveryItem(string component, string category, string owner, string progress, string status)
        {
            Component = component;
            Category = category;
            Owner = owner;
            Progress = progress;
            Status = status;
        }

        public string Component { get; }
        public string Category { get; }
        public string Owner { get; }
        public string Progress { get; }
        public string Status { get; }
    }
}
