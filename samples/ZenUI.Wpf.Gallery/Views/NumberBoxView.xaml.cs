using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class NumberBoxView : UserControl
    {
        public NumberBoxView()
        {
            EditorFeedbackCommand = new DelegateCommand<object>(ShowEditorCommandFeedback);
            InitializeComponent();
        }

        public ICommand EditorFeedbackCommand { get; }

        private void ShowEditorCommandFeedback(object parameter)
        {
            EditorCommandStatus.Text = "已执行命令，参数：" + (parameter ?? "null");
        }
    }
}
