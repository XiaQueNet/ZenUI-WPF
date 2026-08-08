using System.Windows.Markup;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]
#endif

[assembly: XmlnsDefinition(
    "https://zenui.mnorg.cn/xaml/wpf/converters",
    "ZenUI.Wpf.Converters")]
[assembly: XmlnsPrefix(
    "https://zenui.mnorg.cn/xaml/wpf/converters",
    "zenConverters")]
