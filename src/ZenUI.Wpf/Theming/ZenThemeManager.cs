using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ZenUI.Wpf.Theming
{
    /// <summary>
    /// 提供运行时主题切换能力。
    /// </summary>
    public static class ZenThemeManager
    {
        private const string ThemePathPrefix = "/ZenUI.Wpf;component/Themes/";

        /// <summary>
        /// 将主题应用到指定资源字典。系统处于高对比度模式时默认优先使用高对比度主题。
        /// </summary>
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

            RemoveThemeOverrides(resources.MergedDictionaries);
            var effectiveTheme = respectSystemHighContrast && SystemParameters.HighContrast
                ? ZenTheme.HighContrast
                : theme;

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
    }
}
