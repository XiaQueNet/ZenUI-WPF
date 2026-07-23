using System.Windows;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;

using ZenUI.Wpf.Demo.Navigation;
using ZenUI.Wpf.Demo.ViewModels;
using ZenUI.Wpf.Demo.Views;

namespace ZenUI.Wpf.Demo
{
    public partial class App : PrismApplication
    {
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<OverviewView>(NavigationKeys.Overview);
            containerRegistry.RegisterForNavigation<ButtonView>(NavigationKeys.Button);
            containerRegistry.RegisterForNavigation<TextBoxView>(NavigationKeys.TextBox);
            containerRegistry.RegisterForNavigation<PasswordBoxView>(NavigationKeys.PasswordBox);
            containerRegistry.RegisterForNavigation<SwitchView>(NavigationKeys.Switch);
            containerRegistry.RegisterForNavigation<CheckBoxView>(NavigationKeys.CheckBox);
            containerRegistry.RegisterForNavigation<RadioButtonView>(NavigationKeys.RadioButton);
            containerRegistry.RegisterForNavigation<ComboBoxView>(NavigationKeys.ComboBox);
            containerRegistry.RegisterForNavigation<DataGridView>(NavigationKeys.DataGrid);
            containerRegistry.RegisterForNavigation<SliderView>(NavigationKeys.Slider);
            containerRegistry.RegisterForNavigation<ProgressBarView>(NavigationKeys.ProgressBar);
            containerRegistry.RegisterForNavigation<AlertView>(NavigationKeys.Alert);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>()
                .RequestNavigate(RegionNames.ContentRegion, NavigationKeys.Overview);
        }
    }
}
