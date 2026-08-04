using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using NLog;

namespace ZenUI.Wpf.Gallery
{
    /// <summary>
    /// 处理 WPF 消息循环启动前的致命异常。
    /// </summary>
    internal static class StartupExceptionHandler
    {
        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "全局异常边界必须保证日志组件或错误提示失败时不会掩盖原始异常。")]
        internal static void Handle(Exception exception)
        {
            Exception nlogException = null;

            try
            {
                // 此处不使用静态 Logger，避免类型初始化发生在最外层 try 之前。
                var logger = LogManager.GetLogger("Startup");
                logger.Fatal(exception, "应用启动阶段发生未处理异常。");
                LogManager.Flush();
            }
            catch (Exception loggingException)
            {
                nlogException = loggingException;
            }

            if (nlogException != null)
            {
                EmergencyExceptionLogger.TryWrite(exception, nlogException);
            }

            try
            {
                MessageBox.Show(
                    "应用启动失败，详细信息已写入日志文件。",
                    "ZenUI Gallery",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception messageBoxException)
            {
                EmergencyExceptionLogger.TryWrite(exception, messageBoxException);
            }

            try
            {
                LogManager.Shutdown();
            }
            catch (Exception shutdownException)
            {
                EmergencyExceptionLogger.TryWrite(exception, shutdownException);
            }
        }
    }
}
