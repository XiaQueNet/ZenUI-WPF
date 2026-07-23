using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;

namespace ZenUI.Wpf.Demo.ViewModels
{
    public sealed class ProgressBarViewModel : BindableBase
    {
        private double simulatedProgress;
        private bool isRunning;

        public ProgressBarViewModel()
        {
            StartProgressCommand = new DelegateCommand(StartProgress, CanStartProgress);
        }

        public double SimulatedProgress
        {
            get { return simulatedProgress; }
            private set { SetProperty(ref simulatedProgress, value); }
        }

        public DelegateCommand StartProgressCommand { get; }

        private bool CanStartProgress()
        {
            return !isRunning;
        }

        private async void StartProgress()
        {
            isRunning = true;
            StartProgressCommand.RaiseCanExecuteChanged();
            SimulatedProgress = 0;

            try
            {
                for (var progress = 1; progress <= 100; progress++)
                {
                    await Task.Delay(35);
                    SimulatedProgress = progress;
                }
            }
            finally
            {
                isRunning = false;
                StartProgressCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
