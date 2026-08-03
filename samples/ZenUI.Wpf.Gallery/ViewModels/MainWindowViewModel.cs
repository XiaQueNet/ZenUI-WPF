using System.Collections.Generic;
using System.Windows;

using Prism.Mvvm;
using Prism.Navigation.Regions;

using ZenUI.Wpf.Gallery.Navigation;
using ZenUI.Wpf.Theming;

namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private DensityOption selectedDensityOption;
        private MenuItemViewModel selectedMenuItem;
        private ThemeOption selectedThemeOption;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            ThemeOptions = new[]
            {
                new ThemeOption("浅色", ZenTheme.Light),
                new ThemeOption("深色", ZenTheme.Dark),
                new ThemeOption("高对比度", ZenTheme.HighContrast)
            };
            selectedThemeOption = ThemeOptions[0];

            DensityOptions = new[]
            {
                new DensityOption("紧凑", ZenDensity.Compact),
                new DensityOption("标准", ZenDensity.Standard),
                new DensityOption("宽松", ZenDensity.Comfortable)
            };
            selectedDensityOption = DensityOptions[1];

            NavigationGroups = new[]
            {
                new NavigationGroupViewModel(
                    "指南",
                    SelectMenuItem,
                    new MenuItemViewModel("Overview", "概览", NavigationKeys.Overview),
                    new MenuItemViewModel("Design Tokens", "设计令牌", NavigationKeys.Token)),
                new NavigationGroupViewModel(
                    "操作与选择",
                    SelectMenuItem,
                    new MenuItemViewModel("Button", "按钮", NavigationKeys.Button),
                    new MenuItemViewModel("CheckBox", "复选框", NavigationKeys.CheckBox),
                    new MenuItemViewModel("RadioButton", "单选框", NavigationKeys.RadioButton),
                    new MenuItemViewModel("RadioGroup", "单选组", NavigationKeys.RadioGroup),
                    new MenuItemViewModel("Switch", "开关", NavigationKeys.Switch)),
                new NavigationGroupViewModel(
                    "数据输入",
                    SelectMenuItem,
                    new MenuItemViewModel("TextBox", "输入框", NavigationKeys.TextBox),
                    new MenuItemViewModel("PasswordBox", "密码框", NavigationKeys.PasswordBox),
                    new MenuItemViewModel("NumberBox", "数字输入框", NavigationKeys.NumberBox),
                    new MenuItemViewModel("ComboBox", "下拉框", NavigationKeys.ComboBox),
                    new MenuItemViewModel("Slider", "滑块", NavigationKeys.Slider),
                    new MenuItemViewModel("DatePicker", "日期选择器", NavigationKeys.DatePicker),
                    new MenuItemViewModel("TimePicker", "时间选择器", NavigationKeys.TimePicker),
                    new MenuItemViewModel("DateTimePicker", "日期时间选择器", NavigationKeys.DateTimePicker)),
                new NavigationGroupViewModel(
                    "数据展示",
                    SelectMenuItem,
                    new MenuItemViewModel("ListBox", "列表框", NavigationKeys.ListBox),
                    new MenuItemViewModel("DataGrid", "数据表格", NavigationKeys.DataGrid),
                    new MenuItemViewModel("Calendar", "日历", NavigationKeys.Calendar),
                    new MenuItemViewModel("Expander", "折叠面板", NavigationKeys.Expander)),
                new NavigationGroupViewModel(
                    "状态反馈",
                    SelectMenuItem,
                    new MenuItemViewModel("Alert", "状态提示", NavigationKeys.Alert),
                    new MenuItemViewModel("ProgressBar", "进度条", NavigationKeys.ProgressBar),
                    new MenuItemViewModel("Loading", "加载状态", NavigationKeys.Loading)),
                new NavigationGroupViewModel(
                    "浮层与菜单",
                    SelectMenuItem,
                    new MenuItemViewModel("Popover", "气泡提示", NavigationKeys.Popover),
                    new MenuItemViewModel("ContextMenu", "右键菜单", NavigationKeys.ContextMenu))
            };
            selectedMenuItem = NavigationGroups[0].Items[0];
            SynchronizeGroupSelection(selectedMenuItem);
        }

        public IReadOnlyList<NavigationGroupViewModel> NavigationGroups { get; }

        public IReadOnlyList<DensityOption> DensityOptions { get; }

        public IReadOnlyList<ThemeOption> ThemeOptions { get; }

        public MenuItemViewModel SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set
            {
                if (value != null && SetProperty(ref selectedMenuItem, value))
                {
                    SynchronizeGroupSelection(value);
                    ExpandContainingGroup(value);
                    regionManager.RequestNavigate(
                        RegionNames.ContentRegion,
                        value.NavigationTarget);
                }
            }
        }

        public DensityOption SelectedDensityOption
        {
            get { return selectedDensityOption; }
            set
            {
                if (value != null && SetProperty(ref selectedDensityOption, value))
                {
                    ZenDensityManager.ApplyDensity(Application.Current.Resources, value.Density);
                }
            }
        }

        public ThemeOption SelectedThemeOption
        {
            get { return selectedThemeOption; }
            set
            {
                if (value != null && SetProperty(ref selectedThemeOption, value))
                {
                    ZenThemeManager.ApplyTheme(Application.Current.Resources, value.Theme);
                }
            }
        }

        private void ExpandContainingGroup(MenuItemViewModel menuItem)
        {
            foreach (var group in NavigationGroups)
            {
                foreach (var item in group.Items)
                {
                    if (ReferenceEquals(item, menuItem))
                    {
                        group.IsExpanded = true;
                        return;
                    }
                }
            }
        }

        private void SelectMenuItem(MenuItemViewModel menuItem)
        {
            SelectedMenuItem = menuItem;
        }

        private void SynchronizeGroupSelection(MenuItemViewModel menuItem)
        {
            foreach (var group in NavigationGroups)
            {
                group.SelectedItem = Contains(group, menuItem)
                    ? menuItem
                    : null;
            }
        }

        private static bool Contains(
            NavigationGroupViewModel group,
            MenuItemViewModel menuItem)
        {
            foreach (var item in group.Items)
            {
                if (ReferenceEquals(item, menuItem))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class ThemeOption
    {
        public ThemeOption(string displayName, ZenTheme theme)
        {
            DisplayName = displayName;
            Theme = theme;
        }

        public string DisplayName { get; }

        public ZenTheme Theme { get; }
    }

    public sealed class DensityOption
    {
        public DensityOption(string displayName, ZenDensity density)
        {
            DisplayName = displayName;
            Density = density;
        }

        public string DisplayName { get; }

        public ZenDensity Density { get; }
    }
}
