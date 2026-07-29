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
                    new MenuItemViewModel("概览", NavigationKeys.Overview),
                    new MenuItemViewModel("设计 Token", NavigationKeys.Token)),
                new NavigationGroupViewModel(
                    "基础交互",
                    SelectMenuItem,
                    new MenuItemViewModel("按钮  Button", NavigationKeys.Button),
                    new MenuItemViewModel("复选框  CheckBox", NavigationKeys.CheckBox),
                    new MenuItemViewModel("单选框  RadioButton", NavigationKeys.RadioButton),
                    new MenuItemViewModel("单选组  RadioGroup", NavigationKeys.RadioGroup),
                    new MenuItemViewModel("开关  Switch", NavigationKeys.Switch)),
                new NavigationGroupViewModel(
                    "数据输入",
                    SelectMenuItem,
                    new MenuItemViewModel("输入框  TextBox", NavigationKeys.TextBox),
                    new MenuItemViewModel("密码框  PasswordBox", NavigationKeys.PasswordBox),
                    new MenuItemViewModel("数字输入框  NumberBox", NavigationKeys.NumberBox),
                    new MenuItemViewModel("下拉框  ComboBox", NavigationKeys.ComboBox),
                    new MenuItemViewModel("滑块  Slider", NavigationKeys.Slider),
                    new MenuItemViewModel("日期选择器  DatePicker", NavigationKeys.DatePicker)),
                new NavigationGroupViewModel(
                    "数据展示",
                    SelectMenuItem,
                    new MenuItemViewModel("列表框  ListBox", NavigationKeys.ListBox),
                    new MenuItemViewModel("数据表格  DataGrid", NavigationKeys.DataGrid),
                    new MenuItemViewModel("日历  Calendar", NavigationKeys.Calendar),
                    new MenuItemViewModel("折叠面板  Expander", NavigationKeys.Expander)),
                new NavigationGroupViewModel(
                    "反馈",
                    SelectMenuItem,
                    new MenuItemViewModel("状态提示  Alert", NavigationKeys.Alert),
                    new MenuItemViewModel("进度条  ProgressBar", NavigationKeys.ProgressBar)),
                new NavigationGroupViewModel(
                    "浮层与菜单",
                    SelectMenuItem,
                    new MenuItemViewModel("气泡提示  Popover", NavigationKeys.Popover),
                    new MenuItemViewModel("右键菜单  ContextMenu", NavigationKeys.ContextMenu))
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
