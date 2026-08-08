using System.Windows;
using System.Windows.Markup;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]
#endif

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition("https://zenui.mnorg.cn/xaml/wpf", "ZenUI.Wpf.Controls")]
[assembly: XmlnsDefinition("https://zenui.mnorg.cn/xaml/wpf", "ZenUI.Wpf.Theming")]
[assembly: XmlnsPrefix("https://zenui.mnorg.cn/xaml/wpf", "zen")]
