using System.Windows;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;

using ZenUI.Wpf.Gallery.Navigation;
using ZenUI.Wpf.Gallery.ViewModels;
using ZenUI.Wpf.Gallery.Views;

namespace ZenUI.Wpf.Gallery
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
            containerRegistry.RegisterForNavigation<NumberBoxView>(NavigationKeys.NumberBox);
            containerRegistry.RegisterForNavigation<PasswordBoxView>(NavigationKeys.PasswordBox);
            containerRegistry.RegisterForNavigation<SwitchView>(NavigationKeys.Switch);
            containerRegistry.RegisterForNavigation<CheckBoxView>(NavigationKeys.CheckBox);
            containerRegistry.RegisterForNavigation<RadioButtonView>(NavigationKeys.RadioButton);
            containerRegistry.RegisterForNavigation<ComboBoxView>(NavigationKeys.ComboBox);
            containerRegistry.RegisterForNavigation<DatePickerView>(NavigationKeys.DatePicker);
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
