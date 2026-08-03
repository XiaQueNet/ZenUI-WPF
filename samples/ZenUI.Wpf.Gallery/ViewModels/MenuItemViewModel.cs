namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class MenuItemViewModel
    {
        public MenuItemViewModel(
            string controlName,
            string chineseName,
            string navigationTarget)
        {
            ControlName = controlName;
            ChineseName = chineseName;
            NavigationTarget = navigationTarget;
        }

        public string ChineseName { get; }

        public string ControlName { get; }

        public string DisplayName
        {
            get { return ControlName + " " + ChineseName; }
        }

        public string NavigationTarget { get; }
    }
}
