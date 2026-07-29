namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class MenuItemViewModel
    {
        public MenuItemViewModel(string displayName, string navigationTarget)
        {
            DisplayName = displayName;
            NavigationTarget = navigationTarget;
        }

        public string DisplayName { get; }

        public string NavigationTarget { get; }
    }
}
