using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace ZenUI.Wpf.Gallery
{
    /// <summary>
    /// 在 WPF、Prism 或 NLog 无法加载时写入最后一道应急日志。
    /// </summary>
    internal static class EmergencyExceptionLogger
    {
        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "应急日志是最后一道异常边界，写入失败时已没有更安全的恢复方式。")]
        internal static void TryWrite(Exception originalException, Exception handlerException)
        {
            try
            {
                var logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(logsDirectory);

                var fileName = string.Concat(
                    "startup-crash-",
                    DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ".log");
                var logPath = Path.Combine(logsDirectory, fileName);
                var content = new StringBuilder()
                    .Append(DateTime.Now.ToString("O", CultureInfo.InvariantCulture))
                    .AppendLine("|FATAL|应用启动阶段发生未处理异常。")
                    .AppendLine(originalException.ToString());

                if (handlerException != null)
                {
                    content.AppendLine("异常处理器执行时发生异常：")
                        .AppendLine(handlerException.ToString());
                }

                content.AppendLine();
                File.AppendAllText(logPath, content.ToString(), Encoding.UTF8);
            }
            catch (Exception)
            {
                // 应急日志也无法写入时保持静默，避免覆盖最初的启动异常。
            }
        }
    }
}
