using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using NLog;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

using ZenUI.Wpf.Gallery.Navigation;
using ZenUI.Wpf.Gallery.ViewModels;
using ZenUI.Wpf.Gallery.Views;

namespace ZenUI.Wpf.Gallery
{
    public partial class App : PrismApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            // 在 App.xaml 资源初始化前订阅，尽可能早地捕获启动阶段异常。
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Info("ZenUI Gallery 已启动。");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                Logger.Info(
                    CultureInfo.InvariantCulture,
                    "ZenUI Gallery 已退出，退出代码：{ExitCode}。",
                    e.ApplicationExitCode);
                base.OnExit(e);
            }
            finally
            {
                // 解除静态事件订阅，并确保退出前将异步日志写入磁盘。
                DispatcherUnhandledException -= OnDispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
                TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
                LogManager.Shutdown();
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "UI 线程发生未处理异常。");
            LogManager.Flush();

            MessageBox.Show(
                "应用发生异常，详细信息已写入日志。",
                "ZenUI Gallery",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // 异常已统一记录并提示，阻止其继续上抛导致应用直接退出。
            e.Handled = true;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 此事件通常表示进程即将终止，因此使用 Fatal 级别并立即刷新日志。
            if (e.ExceptionObject is Exception exception)
            {
                Logger.Fatal(
                    exception,
                    CultureInfo.InvariantCulture,
                    "非 UI 线程发生未处理异常，进程是否正在终止：{IsTerminating}。",
                    e.IsTerminating);
            }
            else
            {
                Logger.Fatal(
                    CultureInfo.InvariantCulture,
                    "非 UI 线程发生未处理异常：{ExceptionObject}；进程是否正在终止：{IsTerminating}。",
                    e.ExceptionObject,
                    e.IsTerminating);
            }

            LogManager.Flush();
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "异步任务发生未观察异常。");

            // 标记异常已被观察，避免旧版 .NET Framework 的默认终止行为。
            e.SetObserved();
        }

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
            containerRegistry.RegisterForNavigation<TokenView>(NavigationKeys.Token);
            containerRegistry.RegisterForNavigation<ButtonView>(NavigationKeys.Button);
            containerRegistry.RegisterForNavigation<TextBoxView>(NavigationKeys.TextBox);
            containerRegistry.RegisterForNavigation<NumberBoxView>(NavigationKeys.NumberBox);
            containerRegistry.RegisterForNavigation<PasswordBoxView>(NavigationKeys.PasswordBox);
            containerRegistry.RegisterForNavigation<SwitchView>(NavigationKeys.Switch);
            containerRegistry.RegisterForNavigation<CheckBoxView>(NavigationKeys.CheckBox);
            containerRegistry.RegisterForNavigation<RadioButtonView>(NavigationKeys.RadioButton);
            containerRegistry.RegisterForNavigation<RadioGroupView>(NavigationKeys.RadioGroup);
            containerRegistry.RegisterForNavigation<ComboBoxView>(NavigationKeys.ComboBox);
            containerRegistry.RegisterForNavigation<ListBoxView>(NavigationKeys.ListBox);
            containerRegistry.RegisterForNavigation<CalendarView>(NavigationKeys.Calendar);
            containerRegistry.RegisterForNavigation<DatePickerView>(NavigationKeys.DatePicker);
            containerRegistry.RegisterForNavigation<TimePickerView>(NavigationKeys.TimePicker);
            containerRegistry.RegisterForNavigation<DateTimePickerView>(NavigationKeys.DateTimePicker);
            containerRegistry.RegisterForNavigation<DataGridView>(NavigationKeys.DataGrid);
            containerRegistry.RegisterForNavigation<SliderView>(NavigationKeys.Slider);
            containerRegistry.RegisterForNavigation<ProgressBarView>(NavigationKeys.ProgressBar);
            containerRegistry.RegisterForNavigation<LoadingView>(NavigationKeys.Loading);
            containerRegistry.RegisterForNavigation<AlertView>(NavigationKeys.Alert);
            containerRegistry.RegisterForNavigation<ExpanderView>(NavigationKeys.Expander);
            containerRegistry.RegisterForNavigation<PopoverView>(NavigationKeys.Popover);
            containerRegistry.RegisterForNavigation<ContextMenuView>(NavigationKeys.ContextMenu);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>()
                .RequestNavigate(RegionNames.ContentRegion, NavigationKeys.Overview);
        }
    }
}
