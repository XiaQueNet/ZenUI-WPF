using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 与拥有者窗口的屏幕边界保持重合的遮罩对话框宿主。
    /// 使用 ShowDialog 显示以阻止操作拥有者窗口。
    /// </summary>
    public class ZenOverlayDialogWindow : Window
    {
        private const int ExtendedFrameBounds = 9;
        private const uint NoZOrderOrActivation = 0x0004 | 0x0010;
        private Window trackedOwner;

        /// <summary>创建一个由 Owner 指定覆盖目标的遮罩对话框窗口。</summary>
        public ZenOverlayDialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
        }

        /// <summary>初始化窗口句柄后按拥有者窗口的屏幕边界定位遮罩。</summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            trackedOwner = Owner ?? throw new InvalidOperationException("遮罩对话框必须设置 Owner。");
            trackedOwner.LocationChanged += OnOwnerBoundsChanged;
            trackedOwner.SizeChanged += OnOwnerBoundsChanged;
            trackedOwner.StateChanged += OnOwnerBoundsChanged;
            UpdateOwnerBounds();
        }

        /// <summary>关闭时停止跟踪拥有者窗口的位置和尺寸。</summary>
        protected override void OnClosed(EventArgs e)
        {
            if (trackedOwner != null)
            {
                trackedOwner.LocationChanged -= OnOwnerBoundsChanged;
                trackedOwner.SizeChanged -= OnOwnerBoundsChanged;
                trackedOwner.StateChanged -= OnOwnerBoundsChanged;
                trackedOwner = null;
            }

            base.OnClosed(e);
        }

        private void OnOwnerBoundsChanged(object sender, EventArgs e) => UpdateOwnerBounds();

        private void UpdateOwnerBounds()
        {
            var overlayHandle = new WindowInteropHelper(this).Handle;
            var ownerHandle = new WindowInteropHelper(trackedOwner).Handle;
            if (overlayHandle == IntPtr.Zero || ownerHandle == IntPtr.Zero)
            {
                return;
            }

            // GetWindowRect 包含系统不可见的缩放边框，会让遮罩比实际窗口大一圈。
            // DWM 的扩展框架边界对应用户看到的窗口轮廓。
            if (DwmGetWindowAttribute(ownerHandle, ExtendedFrameBounds, out var bounds, Marshal.SizeOf<WindowRect>()) < 0
                || bounds.Right <= bounds.Left || bounds.Bottom <= bounds.Top)
            {
                if (!GetWindowRect(ownerHandle, out bounds))
                {
                    return;
                }
            }

            IntPtr previousDpiContext;
            try
            {
                // DWM 返回物理像素；切换调用线程后，SetWindowPos 也按物理像素解释坐标。
                previousDpiContext = SetThreadDpiAwarenessContext(new IntPtr(-4));
            }
            catch (EntryPointNotFoundException)
            {
                // 较早的 Windows 版本没有按显示器设置线程 DPI 的 API。
                previousDpiContext = IntPtr.Zero;
            }

            try
            {
                SetWindowPos(overlayHandle, IntPtr.Zero, bounds.Left, bounds.Top,
                    bounds.Right - bounds.Left, bounds.Bottom - bounds.Top, NoZOrderOrActivation);
            }
            finally
            {
                if (previousDpiContext != IntPtr.Zero)
                {
                    SetThreadDpiAwarenessContext(previousDpiContext);
                }
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr window, out WindowRect rectangle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr window, int attribute, out WindowRect rectangle, int size);

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
