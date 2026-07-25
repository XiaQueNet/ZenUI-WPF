using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ZenUI.Wpf.Theming
{
    /// <summary>
    /// 提供不影响当前颜色主题的运行时界面密度切换能力。
    /// </summary>
    public static class ZenDensityManager
    {
        private const string DensityPathPrefix = "/ZenUI.Wpf;component/Themes/Density/";

        /// <summary>
        /// 将界面密度配置应用到指定资源字典。
        /// </summary>
        /// <param name="resources">接收界面密度覆盖的资源字典。</param>
        /// <param name="density">要应用的界面密度。</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources"/> 为 <see langword="null"/>。
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="density"/> 不是已定义的 <see cref="ZenDensity"/> 值。
        /// </exception>
        /// <remarks>必须在拥有该资源字典的 UI 线程上调用。</remarks>
        public static void ApplyDensity(ResourceDictionary resources, ZenDensity density)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(resources);
#else
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }
#endif

            string fileName;
            switch (density)
            {
                case ZenDensity.Compact:
                    fileName = "Compact.xaml";
                    break;
                case ZenDensity.Standard:
                    fileName = null;
                    break;
                case ZenDensity.Comfortable:
                    fileName = "Comfortable.xaml";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(density), density, null);
            }

            RemoveDensityOverrides(resources.MergedDictionaries);
            if (fileName == null)
            {
                return;
            }

            resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(DensityPathPrefix + fileName, UriKind.Relative)
            });
        }

        private static void RemoveDensityOverrides(Collection<ResourceDictionary> dictionaries)
        {
            for (var index = dictionaries.Count - 1; index >= 0; index--)
            {
                var source = dictionaries[index].Source?.OriginalString;
                if (source != null &&
                    (source.EndsWith("/Themes/Density/Compact.xaml", StringComparison.OrdinalIgnoreCase) ||
                     source.EndsWith("/Themes/Density/Comfortable.xaml", StringComparison.OrdinalIgnoreCase)))
                {
                    dictionaries.RemoveAt(index);
                }
            }
        }
    }
}
