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
            new TokenGroup("语义颜色", "SemanticColors.xaml"),
            new TokenGroup("组件颜色", "ComponentColors.xaml"),
            new TokenGroup("排版", "Typography.xaml"),
            new TokenGroup("基础尺寸", "Metrics.xaml"),
            new TokenGroup("组件尺寸", "ComponentMetrics.xaml"),
            new TokenGroup("交互状态", "Interaction.xaml")
        };

        public TokenView()
        {
            InitializeComponent();
            DensityDataGrid.ItemsSource = new[]
            {
                new DensityPreviewRow("DataGrid", "表头、行高、单元格内边距"),
                new DensityPreviewRow("Calendar", "展开上方日期选择器查看日期格")
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
                content.Children.Add(new TextBlock
                {
                    Text = group.DisplayName,
                    Style = (Style)FindResource("SectionTitleStyle")
                });
                var tokens = new WrapPanel();
                foreach (var key in dictionary.Keys
                    .Cast<object>()
                    .OrderBy(item => item.ToString(), StringComparer.Ordinal))
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
                Width = 330,
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
            public TokenGroup(string displayName, string fileName)
            {
                DisplayName = displayName;
                FileName = fileName;
            }

            public string DisplayName { get; }

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
