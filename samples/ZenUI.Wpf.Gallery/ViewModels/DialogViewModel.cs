using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using ZenUI.Wpf.Gallery.Views;

namespace ZenUI.Wpf.Gallery.ViewModels
{
    public sealed class DialogViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private string resultMessage = "尚未打开对话框。";
        private int mainWindowClickCount;

        public DialogViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            ShowDialogCommand = new DelegateCommand(ShowDialog);
            ShowConfirmOnlyCommand = new DelegateCommand(ShowConfirmOnly);
            ShowCancelOnlyCommand = new DelegateCommand(ShowCancelOnly);
            ShowContentOnlyCommand = new DelegateCommand(ShowContentOnly);
            ShowNonModalCommand = new DelegateCommand(ShowNonModal);
            ShowOverlayCommand = new DelegateCommand(ShowOverlay);
            IncrementCounterCommand = new DelegateCommand(IncrementCounter);
        }

        public DelegateCommand ShowDialogCommand { get; }

        public DelegateCommand ShowConfirmOnlyCommand { get; }

        public DelegateCommand ShowCancelOnlyCommand { get; }

        public DelegateCommand ShowContentOnlyCommand { get; }

        public DelegateCommand ShowNonModalCommand { get; }

        public DelegateCommand ShowOverlayCommand { get; }

        public DelegateCommand IncrementCounterCommand { get; }

        public int MainWindowClickCount
        {
            get { return mainWindowClickCount; }
            private set { SetProperty(ref mainWindowClickCount, value); }
        }

        public string ResultMessage
        {
            get { return resultMessage; }
            private set { SetProperty(ref resultMessage, value); }
        }

        private void ShowDialog()
        {
            OpenDialog(nameof(ConfirmDialogView), "双按钮弹窗 · 440 × 240");
        }

        private void ShowConfirmOnly()
        {
            OpenDialog(nameof(ConfirmOnlyDialogView), "紧凑弹窗 · 380 × 220");
        }

        private void ShowCancelOnly()
        {
            OpenDialog(nameof(CancelOnlyDialogView), "较大弹窗 · 600 × 320");
        }

        private void ShowContentOnly()
        {
            OpenDialog(nameof(ContentOnlyDialogView), "无底部操作区、不可拖动");
        }

        private void ShowNonModal()
        {
            var parameters = new DialogParameters
            {
                { "title", "非模态弹窗" },
                { "message", "关闭前可以继续操作主窗口。" }
            };
            dialogService.Show(nameof(NonModalDialogView), parameters, OnDialogClosed);
        }

        private void ShowOverlay()
        {
            var parameters = new DialogParameters
            {
                { "title", "遮罩弹窗" },
                { "message", "遮罩与主窗口重叠，弹窗内容固定在遮罩中央。" }
            };
            dialogService.ShowDialog(nameof(OverlayDialogView), parameters, OnDialogClosed, nameof(GalleryOverlayDialogWindow));
        }

        private void IncrementCounter()
        {
            MainWindowClickCount++;
        }

        private void OpenDialog(string viewName, string title)
        {
            var parameters = new DialogParameters
            {
                { "title", title },
                { "message", "点击底部按钮查看弹窗返回结果。此操作仅用于演示。" }
            };
            dialogService.ShowDialog(viewName, parameters, OnDialogClosed);
        }

        private void OnDialogClosed(IDialogResult result)
        {
            if (result == null)
            {
                ResultMessage = "已从右上角关闭：未确认操作。";
                return;
            }

            switch (result.Result)
            {
                case ButtonResult.OK:
                    ResultMessage = "已确认：返回 OK。";
                    break;
                case ButtonResult.Cancel:
                    ResultMessage = "已取消：返回 Cancel。";
                    break;
                default:
                    ResultMessage = "已从右上角关闭：未确认操作。";
                    break;
            }
        }
    }
}
