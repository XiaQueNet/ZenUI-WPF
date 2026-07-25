using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace ZenUI.Wpf.Theming
{
    /// <summary>
    /// 提供运行时主题切换能力。
    /// </summary>
    public static class ZenThemeManager
    {
        private const string ThemePathPrefix = "/ZenUI.Wpf;component/Themes/";
        private static readonly object SystemThemeSyncRoot = new object();
        private static readonly List<SystemThemeRegistration> SystemThemeRegistrations =
            new List<SystemThemeRegistration>();
        private static bool isSystemThemeListenerAttached;

        /// <summary>
        /// 将主题应用到指定资源字典，并默认持续跟随系统高对比度状态。
        /// </summary>
        /// <param name="resources">接收主题资源的资源字典。</param>
        /// <param name="theme">要应用的主题。</param>
        /// <param name="respectSystemHighContrast">
        /// 是否在 Windows 启用高对比度时优先应用高对比度主题。
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources"/> 为 <see langword="null"/>。
        /// </exception>
        /// <remarks>必须在拥有该资源字典的 UI 线程上调用。</remarks>
        public static void ApplyTheme(ResourceDictionary resources, ZenTheme theme, bool respectSystemHighContrast = true)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(resources);
#else
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }
#endif

            if (respectSystemHighContrast)
            {
                TrackSystemHighContrast(resources, theme);
            }
            else
            {
                StopTrackingSystemHighContrast(resources);
            }

            ApplyEffectiveTheme(
                resources,
                theme,
                respectSystemHighContrast && SystemParameters.HighContrast);
        }

        internal static void ApplySystemHighContrastState(bool highContrast)
        {
            List<SystemThemeRegistration> registrations;
            lock (SystemThemeSyncRoot)
            {
                for (var index = SystemThemeRegistrations.Count - 1; index >= 0; index--)
                {
                    var registration = SystemThemeRegistrations[index];
                    if (!registration.Resources.TryGetTarget(out _) ||
                        registration.Dispatcher.HasShutdownStarted ||
                        registration.Dispatcher.HasShutdownFinished)
                    {
                        SystemThemeRegistrations.RemoveAt(index);
                    }
                }

                registrations = new List<SystemThemeRegistration>(SystemThemeRegistrations);
            }

            foreach (var registration in registrations)
            {
                if (!registration.Resources.TryGetTarget(out var resources))
                {
                    continue;
                }

                if (registration.Dispatcher.HasShutdownStarted ||
                    registration.Dispatcher.HasShutdownFinished)
                {
                    continue;
                }

                if (registration.Dispatcher.CheckAccess())
                {
                    ApplyEffectiveTheme(resources, registration.Theme, highContrast);
                }
                else
                {
                    registration.Dispatcher.BeginInvoke(
                        DispatcherPriority.DataBind,
                        new Action(
                            () =>
                            {
                                if (registration.Resources.TryGetTarget(out var target))
                                {
                                    ApplyEffectiveTheme(target, registration.Theme, highContrast);
                                }
                            }));
                }
            }
        }

        private static void TrackSystemHighContrast(ResourceDictionary resources, ZenTheme theme)
        {
            lock (SystemThemeSyncRoot)
            {
                foreach (var registration in SystemThemeRegistrations)
                {
                    if (registration.Resources.TryGetTarget(out var target) &&
                        ReferenceEquals(target, resources))
                    {
                        registration.Theme = theme;
                        registration.Dispatcher = Dispatcher.CurrentDispatcher;
                        return;
                    }
                }

                SystemThemeRegistrations.Add(
                    new SystemThemeRegistration(
                        resources,
                        theme,
                        Dispatcher.CurrentDispatcher));
                if (!isSystemThemeListenerAttached)
                {
                    SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
                    isSystemThemeListenerAttached = true;
                }
            }
        }

        private static void StopTrackingSystemHighContrast(ResourceDictionary resources)
        {
            lock (SystemThemeSyncRoot)
            {
                for (var index = SystemThemeRegistrations.Count - 1; index >= 0; index--)
                {
                    if (!SystemThemeRegistrations[index].Resources.TryGetTarget(out var target) ||
                        ReferenceEquals(target, resources))
                    {
                        SystemThemeRegistrations.RemoveAt(index);
                    }
                }
            }
        }

        private static void OnSystemParametersChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) ||
                string.Equals(
                    e.PropertyName,
                    nameof(SystemParameters.HighContrast),
                    StringComparison.Ordinal))
            {
                ApplySystemHighContrastState(SystemParameters.HighContrast);
            }
        }

        private static void ApplyEffectiveTheme(
            ResourceDictionary resources,
            ZenTheme requestedTheme,
            bool highContrast)
        {
            RemoveThemeOverrides(resources.MergedDictionaries);
            var effectiveTheme = highContrast ? ZenTheme.HighContrast : requestedTheme;
            if (effectiveTheme == ZenTheme.Light)
            {
                return;
            }

            resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    ThemePathPrefix + (effectiveTheme == ZenTheme.Dark ? "Dark.xaml" : "HighContrast.xaml"),
                    UriKind.Relative)
            });
        }

        private static void RemoveThemeOverrides(Collection<ResourceDictionary> dictionaries)
        {
            for (var index = dictionaries.Count - 1; index >= 0; index--)
            {
                var source = dictionaries[index].Source?.OriginalString;
                if (source != null &&
                    (source.EndsWith("/Themes/Dark.xaml", StringComparison.OrdinalIgnoreCase) ||
                     source.EndsWith("/Themes/HighContrast.xaml", StringComparison.OrdinalIgnoreCase)))
                {
                    dictionaries.RemoveAt(index);
                }
            }
        }

        private sealed class SystemThemeRegistration
        {
            public SystemThemeRegistration(
                ResourceDictionary resources,
                ZenTheme theme,
                Dispatcher dispatcher)
            {
                Resources = new WeakReference<ResourceDictionary>(resources);
                Theme = theme;
                Dispatcher = dispatcher;
            }

            public WeakReference<ResourceDictionary> Resources { get; }

            public ZenTheme Theme { get; set; }

            public Dispatcher Dispatcher { get; set; }
        }
    }
}
