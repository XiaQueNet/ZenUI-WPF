using System;
using System.Windows.Threading;

using Prism.Mvvm;
using Prism.Navigation.Regions;

using ZenUI.Wpf.PosDemo.Navigation;

namespace ZenUI.Wpf.PosDemo.ViewModels
{
    public sealed class LoadingViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly DispatcherTimer loadingTimer;
        private int loadingProgress;
        private string loadingMessage = "正在初始化本地配置…";

        public LoadingViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            loadingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            loadingTimer.Tick += OnLoadingTimerTick;
            loadingTimer.Start();
        }

        public int LoadingProgress
        {
            get { return loadingProgress; }
            private set { SetProperty(ref loadingProgress, value); }
        }

        public string LoadingMessage
        {
            get { return loadingMessage; }
            private set { SetProperty(ref loadingMessage, value); }
        }

        private void OnLoadingTimerTick(object sender, EventArgs e)
        {
            if (LoadingProgress >= 100)
            {
                loadingTimer.Stop();
                loadingTimer.Tick -= OnLoadingTimerTick;
                regionManager.RequestNavigate(RegionNames.MainRegion, NavigationKeys.Pos);
                return;
            }

            LoadingProgress += 10;
            if (LoadingProgress < 35)
            {
                LoadingMessage = "正在读取门店和收银员配置…";
            }
            else if (LoadingProgress < 65)
            {
                LoadingMessage = "正在加载商品与价格数据…";
            }
            else if (LoadingProgress < 90)
            {
                LoadingMessage = "正在检查订单和设备状态…";
            }
            else
            {
                LoadingMessage = "准备完成，即将进入收银台…";
            }
        }
    }
}
