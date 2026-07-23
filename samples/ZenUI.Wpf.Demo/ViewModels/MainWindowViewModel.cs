using System.Collections.ObjectModel;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

using ZenUI.Wpf.Demo.Navigation;

namespace ZenUI.Wpf.Demo.ViewModels
{
    public sealed class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("概览", NavigationKeys.Overview, true),
                new MenuItemViewModel("按钮  Button", NavigationKeys.Button),
                new MenuItemViewModel("输入框  TextBox", NavigationKeys.TextBox),
                new MenuItemViewModel("密码框  PasswordBox", NavigationKeys.PasswordBox),
                new MenuItemViewModel("开关  Switch", NavigationKeys.Switch),
                new MenuItemViewModel("复选框  CheckBox", NavigationKeys.CheckBox),
                new MenuItemViewModel("单选框  RadioButton", NavigationKeys.RadioButton),
                new MenuItemViewModel("下拉框  ComboBox", NavigationKeys.ComboBox),
                new MenuItemViewModel("数据表格  DataGrid", NavigationKeys.DataGrid),
                new MenuItemViewModel("滑块  Slider", NavigationKeys.Slider),
                new MenuItemViewModel("进度条  ProgressBar", NavigationKeys.ProgressBar),
                new MenuItemViewModel("状态提示  Alert", NavigationKeys.Alert)
            };

            NavigateCommand = new DelegateCommand<MenuItemViewModel>(Navigate, item => item != null);
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        public DelegateCommand<MenuItemViewModel> NavigateCommand { get; }

        private void Navigate(MenuItemViewModel menuItem)
        {
            foreach (var item in MenuItems)
            {
                item.IsSelected = ReferenceEquals(item, menuItem);
            }

            regionManager.RequestNavigate(RegionNames.ContentRegion, menuItem.NavigationTarget);
        }
    }
}
