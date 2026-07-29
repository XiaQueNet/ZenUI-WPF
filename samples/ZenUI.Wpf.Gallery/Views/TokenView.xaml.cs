using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class TokenView : UserControl
    {
        private static readonly TokenGroup[] TokenGroups =
        {
            new TokenGroup("语义颜色", "品牌、表面、文本和状态反馈的主题语义。", "SemanticColors.xaml"),
            new TokenGroup("组件颜色", "面向具体控件状态的背景、边框与前景映射。", "ComponentColors.xaml"),
            new TokenGroup("排版", "统一显示、标题、正文与辅助文本的字号和行高。", "Typography.xaml"),
            new TokenGroup("基础尺寸", "构建设计节奏的间距、圆角、边框和通用尺寸。", "Metrics.xaml"),
            new TokenGroup("组件尺寸", "随 Density 切换的控件高度、内边距与复合尺寸。", "ComponentMetrics.xaml"),
            new TokenGroup("交互状态", "统一悬停、按下、焦点、选中与禁用反馈。", "Interaction.xaml")
        };

        public TokenView()
        {
            InitializeComponent();
            DensityDataGrid.ItemsSource = new[]
            {
                new DensityPreviewRow("输入控件", "最小高度与内边距"),
                new DensityPreviewRow("列表与弹层", "项目及容器间距"),
                new DensityPreviewRow("DataGrid", "表头、行高、单元格内边距"),
                new DensityPreviewRow("Calendar", "日期格、导航与整体尺寸")
            };
            BuildTokenSections();
        }

        private void BuildTokenSections()
        {
            foreach (var group in TokenGroups)
            {
                var dictionary = new ResourceDictionary
                {
                    Source = new Uri(
                        "/ZenUI.Wpf;component/Themes/Tokens/" + group.FileName,
                        UriKind.Relative)
                };
                var section = new Border
                {
                    Style = (Style)FindResource("GalleryCardStyle")
                };
                var content = new StackPanel();
                var keys = dictionary.Keys
                    .Cast<object>()
                    .OrderBy(item => item.ToString(), StringComparer.Ordinal)
                    .ToArray();
                var heading = new Grid
                {
                    Margin = new Thickness(0, 0, 0, 5)
                };
                heading.ColumnDefinitions.Add(new ColumnDefinition());
                heading.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var title = new TextBlock
                {
                    Text = group.DisplayName,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                title.SetResourceReference(TextBlock.FontSizeProperty, "ZenFontSizeSubtitle");
                title.SetResourceReference(TextBlock.ForegroundProperty, "ZenTextPrimaryBrush");
                heading.Children.Add(title);

                var sourceBadge = new Border
                {
                    Padding = new Thickness(9, 4, 9, 4),
                    CornerRadius = new CornerRadius(10)
                };
                sourceBadge.SetResourceReference(Border.BackgroundProperty, "ZenInfoLightBrush");
                var sourceText = new TextBlock
                {
                    Text = group.FileName,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11
                };
                sourceText.SetResourceReference(TextBlock.ForegroundProperty, "ZenPrimaryBrush");
                sourceBadge.Child = sourceText;
                Grid.SetColumn(sourceBadge, 1);
                heading.Children.Add(sourceBadge);
                content.Children.Add(heading);

                var description = new TextBlock
                {
                    Text = group.Description + "  ·  " + keys.Length + " Tokens",
                    Margin = new Thickness(0, 0, 0, 18),
                    TextWrapping = TextWrapping.Wrap
                };
                description.SetResourceReference(TextBlock.ForegroundProperty, "ZenTextSecondaryBrush");
                description.SetResourceReference(TextBlock.FontSizeProperty, "ZenFontSizeBodySmall");
                content.Children.Add(description);

                var tokens = new WrapPanel();
                foreach (var key in keys)
                {
                    tokens.Children.Add(CreateTokenCard(key, dictionary[key] is Brush));
                }

                content.Children.Add(tokens);
                section.Child = content;
                TokenSections.Children.Add(section);
            }
        }

        private Border CreateTokenCard(object key, bool isBrush)
        {
            var card = new Border
            {
                Width = 276,
                MinHeight = 68,
                Margin = new Thickness(0, 0, 12, 12),
                Padding = new Thickness(12),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(7)
            };
            card.SetResourceReference(Border.BackgroundProperty, "ZenSurfaceMutedBrush");
            card.SetResourceReference(Border.BorderBrushProperty, "ZenDividerBrush");
            var layout = new Grid();
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            layout.ColumnDefinitions.Add(new ColumnDefinition());

            var swatch = new Border
            {
                Width = 34,
                Height = 34,
                Margin = new Thickness(0, 0, 12, 0),
                VerticalAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Visibility = isBrush ? Visibility.Visible : Visibility.Collapsed
            };
            swatch.SetResourceReference(Border.BorderBrushProperty, "ZenBorderBrush");
            if (isBrush)
            {
                swatch.SetResourceReference(Border.BackgroundProperty, key);
            }

            layout.Children.Add(swatch);

            var text = new StackPanel();
            Grid.SetColumn(text, 1);
            var keyText = new TextBlock
            {
                Text = key.ToString(),
                FontWeight = FontWeights.SemiBold,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            keyText.SetResourceReference(TextBlock.ForegroundProperty, "ZenTextPrimaryBrush");
            text.Children.Add(keyText);
            var value = new TextBlock
            {
                Margin = new Thickness(0, 5, 0, 0),
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            value.SetResourceReference(TextBlock.ForegroundProperty, "ZenTextSecondaryBrush");
            value.SetResourceReference(FrameworkElement.TagProperty, key);
            value.SetBinding(
                TextBlock.TextProperty,
                new Binding(nameof(Tag))
                {
                    Source = value,
                    StringFormat = "{0}"
                });
            text.Children.Add(value);
            layout.Children.Add(text);
            card.Child = layout;
            return card;
        }

        private sealed class TokenGroup
        {
            public TokenGroup(string displayName, string description, string fileName)
            {
                DisplayName = displayName;
                Description = description;
                FileName = fileName;
            }

            public string DisplayName { get; }

            public string Description { get; }

            public string FileName { get; }
        }

        private sealed class DensityPreviewRow
        {
            public DensityPreviewRow(string name, string description)
            {
                Name = name;
                Description = description;
            }

            public string Name { get; }

            public string Description { get; }
        }
    }
}
