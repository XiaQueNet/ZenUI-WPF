using System;
using System.Collections.Generic;

using Prism.Mvvm;

namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class NavigationGroupViewModel : BindableBase
    {
        private readonly Action<MenuItemViewModel> selectionChanged;
        private bool isExpanded;
        private MenuItemViewModel selectedItem;

        public NavigationGroupViewModel(
            string title,
            Action<MenuItemViewModel> selectionChanged,
            params MenuItemViewModel[] items)
        {
            Title = title;
            Items = items;
            this.selectionChanged = selectionChanged;
            isExpanded = true;
        }

        public string Title { get; }

        public IReadOnlyList<MenuItemViewModel> Items { get; }

        public MenuItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (SetProperty(ref selectedItem, value) && value != null)
                {
                    selectionChanged(value);
                }
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }
    }
}
