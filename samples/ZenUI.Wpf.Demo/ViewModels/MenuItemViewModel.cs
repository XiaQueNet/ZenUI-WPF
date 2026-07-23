using Prism.Mvvm;

namespace ZenUI.Wpf.Demo.ViewModels
{
    public sealed class MenuItemViewModel : BindableBase
    {
        private bool isSelected;

        public MenuItemViewModel(string displayName, string navigationTarget, bool isSelected = false)
        {
            DisplayName = displayName;
            NavigationTarget = navigationTarget;
            this.isSelected = isSelected;
        }

        public string DisplayName { get; }

        public string NavigationTarget { get; }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
    }
}
