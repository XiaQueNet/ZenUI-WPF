using Prism.Mvvm;

namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class SliderViewModel : BindableBase
    {
        private double value = 64;

        public double Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }
    }
}
