using System;
using System.Runtime.CompilerServices;

namespace ZenUI.Wpf.Gallery
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                RunApplication();
            }
            catch (Exception exception)
            {
                try
                {
                    // 正常情况下仅由 NLog 记录；异常处理器失败时再写应急日志。
                    StartupExceptionHandler.Handle(exception);
                }
                catch (Exception handlerException)
                {
                    EmergencyExceptionLogger.TryWrite(exception, handlerException);
                }

                Environment.ExitCode = 1;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RunApplication()
        {
            // 将 WPF/Prism 依赖隔离到第二层，确保程序集加载发生在外层 try 生效之后。
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
