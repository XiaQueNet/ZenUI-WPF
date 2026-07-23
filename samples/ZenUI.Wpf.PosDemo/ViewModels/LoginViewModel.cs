using System.Windows;

using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.PosDemo.ViewModels
{
    public sealed class LoginViewModel : BindableBase, IDialogAware
    {
        private string account = "zenui@mnorg.com";
        private string phone = "188 8888 8888";
        private string captchaInput;
        private string captchaText = "ZEN8";
        private string verificationCode;
        private string sendCodeText = "发送验证码";
        private bool canSendCode = true;
        private string statusMessage;
        private AlertVariant statusVariant;
        private Visibility statusVisibility = Visibility.Collapsed;
        private Visibility accountPanelVisibility = Visibility.Visible;
        private Visibility codePanelVisibility = Visibility.Collapsed;
        private ButtonAppearance accountTabAppearance = ButtonAppearance.Filled;
        private ButtonAppearance codeTabAppearance = ButtonAppearance.Text;
        private int captchaIndex;

        public LoginViewModel()
        {
            UseAccountLoginCommand = new DelegateCommand(UseAccountLogin);
            UseCodeLoginCommand = new DelegateCommand(UseCodeLogin);
            LoginCommand = new DelegateCommand(Login);
            CancelCommand = new DelegateCommand(Cancel);
            RefreshCaptchaCommand = new DelegateCommand(RefreshCaptcha);
            SendCodeCommand = new DelegateCommand(SendCode);
            ForgotPasswordCommand = new DelegateCommand(ForgotPassword);
            CopySerialCommand = new DelegateCommand(CopySerial);
        }

        public DialogCloseListener RequestClose { get; }

        public DelegateCommand UseAccountLoginCommand { get; }

        public DelegateCommand UseCodeLoginCommand { get; }

        public DelegateCommand LoginCommand { get; }

        public DelegateCommand CancelCommand { get; }

        public DelegateCommand RefreshCaptchaCommand { get; }

        public DelegateCommand SendCodeCommand { get; }

        public DelegateCommand ForgotPasswordCommand { get; }

        public DelegateCommand CopySerialCommand { get; }

        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }

        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        public string VerificationCode
        {
            get { return verificationCode; }
            set { SetProperty(ref verificationCode, value); }
        }

        public string CaptchaInput
        {
            get { return captchaInput; }
            set { SetProperty(ref captchaInput, value); }
        }

        public string CaptchaText
        {
            get { return captchaText; }
            private set { SetProperty(ref captchaText, value); }
        }

        public string SendCodeText
        {
            get { return sendCodeText; }
            private set { SetProperty(ref sendCodeText, value); }
        }

        public bool CanSendCode
        {
            get { return canSendCode; }
            private set { SetProperty(ref canSendCode, value); }
        }

        public string StatusMessage
        {
            get { return statusMessage; }
            private set { SetProperty(ref statusMessage, value); }
        }

        public AlertVariant StatusVariant
        {
            get { return statusVariant; }
            private set { SetProperty(ref statusVariant, value); }
        }

        public Visibility StatusVisibility
        {
            get { return statusVisibility; }
            private set { SetProperty(ref statusVisibility, value); }
        }

        public Visibility AccountPanelVisibility
        {
            get { return accountPanelVisibility; }
            private set { SetProperty(ref accountPanelVisibility, value); }
        }

        public Visibility CodePanelVisibility
        {
            get { return codePanelVisibility; }
            private set { SetProperty(ref codePanelVisibility, value); }
        }

        public ButtonAppearance AccountTabAppearance
        {
            get { return accountTabAppearance; }
            private set { SetProperty(ref accountTabAppearance, value); }
        }

        public ButtonAppearance CodeTabAppearance
        {
            get { return codeTabAppearance; }
            private set { SetProperty(ref codeTabAppearance, value); }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            HideStatus();
        }

        private void UseAccountLogin()
        {
            AccountPanelVisibility = Visibility.Visible;
            CodePanelVisibility = Visibility.Collapsed;
            AccountTabAppearance = ButtonAppearance.Filled;
            CodeTabAppearance = ButtonAppearance.Text;
            HideStatus();
        }

        private void UseCodeLogin()
        {
            AccountPanelVisibility = Visibility.Collapsed;
            CodePanelVisibility = Visibility.Visible;
            AccountTabAppearance = ButtonAppearance.Text;
            CodeTabAppearance = ButtonAppearance.Filled;
            HideStatus();
        }

        private void Login()
        {
            RequestClose.Invoke(ButtonResult.OK);
        }

        private void Cancel()
        {
            RequestClose.Invoke(ButtonResult.Cancel);
        }

        private void SendCode()
        {
            VerificationCode = "888888";
            SendCodeText = "验证码已填入";
            CanSendCode = false;
            ShowStatus("演示验证码 888888 已自动填入。", AlertVariant.Info);
        }

        private void RefreshCaptcha()
        {
            var captchaValues = new[] { "ZEN8", "POS6", "UI24", "WPF7" };
            captchaIndex = (captchaIndex + 1) % captchaValues.Length;
            CaptchaText = captchaValues[captchaIndex];
            CaptchaInput = string.Empty;
            ShowStatus("图形验证码已刷新（模拟）。", AlertVariant.Info);
        }

        private void ForgotPassword()
        {
            ShowStatus("已打开模拟找回流程；演示模式不会发送任何请求。", AlertVariant.Info);
        }

        private static void CopySerial()
        {
            Clipboard.SetText("ZEN-POS-001");
        }

        private void ShowStatus(string message, AlertVariant variant)
        {
            StatusMessage = message;
            StatusVariant = variant;
            StatusVisibility = Visibility.Visible;
        }

        private void HideStatus()
        {
            StatusVisibility = Visibility.Collapsed;
        }
    }
}
