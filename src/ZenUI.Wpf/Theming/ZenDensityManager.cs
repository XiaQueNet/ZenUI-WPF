using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ZenUI.Wpf.Theming
{
    /// <summary>
    /// Provides runtime density switching without changing the active color theme.
    /// </summary>
    public static class ZenDensityManager
    {
        private const string DensityPathPrefix = "/ZenUI.Wpf;component/Themes/Density/";

        /// <summary>
        /// Applies a density profile to the specified resource dictionary.
        /// </summary>
        /// <param name="resources">The resource dictionary that receives the density override.</param>
        /// <param name="density">The density profile to apply.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="density"/> is not a defined <see cref="ZenDensity"/> value.
        /// </exception>
        /// <remarks>Call this method on the UI thread that owns the resource dictionary.</remarks>
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
