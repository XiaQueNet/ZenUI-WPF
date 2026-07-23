using System.Windows;

using Prism.DryIoc;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Navigation.Regions;

using ZenUI.Wpf.PosDemo.Navigation;
using ZenUI.Wpf.PosDemo.ViewModels;
using ZenUI.Wpf.PosDemo.Views;

namespace ZenUI.Wpf.PosDemo
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            var loginResult = ButtonResult.None;
            Container.Resolve<IDialogService>()
                .ShowDialog("LoginView", result => loginResult = result.Result);

            if (loginResult == ButtonResult.OK)
            {
                MainWindow = shell;
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                shell.Show();
                Container.Resolve<IRegionManager>()
                    .RequestNavigate(RegionNames.MainRegion, NavigationKeys.Loading);
                return;
            }

            Shutdown();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>("LoginView");
            containerRegistry.RegisterForNavigation<LoadingView, LoadingViewModel>(NavigationKeys.Loading);
            containerRegistry.RegisterForNavigation<PosView, PosViewModel>(NavigationKeys.Pos);
        }
    }
}
