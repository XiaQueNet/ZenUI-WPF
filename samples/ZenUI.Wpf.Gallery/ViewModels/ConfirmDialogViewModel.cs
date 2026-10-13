using System;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class ConfirmDialogViewModel : BindableBase, IDialogAware
    {
        private string message;
        private string title = "对话框";

        public ConfirmDialogViewModel()
        {
            SaveCommand = new DelegateCommand(() => Close(ButtonResult.OK));
            DiscardCommand = new DelegateCommand(() => Close(ButtonResult.Cancel));
        }

        public string Title
        {
            get { return title; }
            private set { SetProperty(ref title, value); }
        }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand DiscardCommand { get; }

        public string Message
        {
            get { return message; }
            private set { SetProperty(ref message, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
            if (parameters.ContainsKey("title"))
            {
                Title = parameters.GetValue<string>("title");
            }
        }

        private void Close(ButtonResult result)
        {
            RequestClose?.Invoke(new DialogResult(result));
        }
    }
}
