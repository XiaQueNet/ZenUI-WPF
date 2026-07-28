using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class DataGridTests
    {
        [TestMethod]
        public void DataGridTemplateDisplaysHeadersAndEmptyContent()
        {
            var dataGrid = new ZenDataGrid
            {
                EmptyContent = "没有数据",
                Height = 120
            };
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "名称",
                Binding = new Binding("Name")
            });
            var window = CreateTestWindow(dataGrid, 240, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                var scrollViewer = dataGrid.Template.FindName("DG_ScrollViewer", dataGrid) as ScrollViewer;
                Assert.IsNotNull(scrollViewer);
                scrollViewer.ApplyTemplate();
                var columnHeaders = scrollViewer.Template.FindName(
                    "PART_ColumnHeadersPresenter",
                    scrollViewer) as DataGridColumnHeadersPresenter;
                Assert.IsNotNull(columnHeaders);
                Assert.AreEqual(Visibility.Visible, columnHeaders.Visibility);
                Assert.IsGreaterThan(0d, columnHeaders.ActualHeight);

                var emptyPresenter = dataGrid.Template.FindName("EmptyPresenter", dataGrid) as ContentControl;
                Assert.IsNotNull(emptyPresenter);
                Assert.AreEqual("没有数据", emptyPresenter.Content);
                Assert.AreEqual(Visibility.Visible, emptyPresenter.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridAppliesColumnHeaderBrushes()
        {
            var headerBackground = new SolidColorBrush(Color.FromRgb(0x12, 0x34, 0x56));
            var headerForeground = new SolidColorBrush(Color.FromRgb(0xFE, 0xDC, 0xBA));
            var dataGrid = new ZenDataGrid
            {
                AutoGenerateColumns = false,
                ColumnHeaderBackground = headerBackground,
                ColumnHeaderForeground = headerForeground,
                HeadersVisibility = DataGridHeadersVisibility.All,
                Height = 120,
                ItemsSource = new[] { new { Name = "成员" } },
                RowHeaderWidth = 32
            };
            var column = new ZenDataGridTextColumn
            {
                Binding = new Binding("Name"),
                CellHorizontalContentAlignment = HorizontalAlignment.Right,
                CellVerticalContentAlignment = VerticalAlignment.Bottom,
                Header = "名称",
                HeaderHorizontalContentAlignment = HorizontalAlignment.Center,
                HeaderVerticalContentAlignment = VerticalAlignment.Bottom
            };
            dataGrid.Columns.Add(column);
            var window = CreateTestWindow(dataGrid, 240, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                var header = FindVisualDescendants<DataGridColumnHeader>(dataGrid)
                    .FirstOrDefault(candidate => candidate.Column == dataGrid.Columns[0]);
                Assert.IsNotNull(header);
                Assert.AreSame(headerBackground, header.Background);
                Assert.AreSame(headerForeground, header.Foreground);
                Assert.AreEqual(HorizontalAlignment.Center, header.HorizontalContentAlignment);
                Assert.AreEqual(VerticalAlignment.Bottom, header.VerticalContentAlignment);

                var cell = FindVisualDescendants<DataGridCell>(dataGrid)
                    .FirstOrDefault(candidate => candidate.Column == column);
                Assert.IsNotNull(cell);
                Assert.AreEqual(HorizontalAlignment.Right, cell.HorizontalContentAlignment);
                Assert.AreEqual(VerticalAlignment.Bottom, cell.VerticalContentAlignment);
                cell.ApplyTemplate();
                var contentPresenter = FindVisualDescendant<ContentPresenter>(cell);
                Assert.IsNotNull(contentPresenter);
                Assert.AreEqual(HorizontalAlignment.Right, contentPresenter.HorizontalAlignment);
                Assert.AreEqual(VerticalAlignment.Bottom, contentPresenter.VerticalAlignment);

                var scrollViewer = dataGrid.Template.FindName("DG_ScrollViewer", dataGrid) as ScrollViewer;
                Assert.IsNotNull(scrollViewer);
                scrollViewer.ApplyTemplate();
                var selectAllButton = scrollViewer.Template.FindName(
                    "PART_SelectAllButton",
                    scrollViewer) as Button;
                Assert.IsNotNull(selectAllButton);
                Assert.AreSame(headerBackground, selectAllButton.Background);
                Assert.AreSame(headerForeground, selectAllButton.Foreground);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridPreservesSelectionLayoutAndVirtualizationContracts()
        {
            var rows = Enumerable.Range(0, 1000)
                .Select(index => new EditableRow(index, "成员 " + index))
                .ToList();
            var grid = CreateAdvancedDataGrid(rows, out _);
            var window = CreateTestWindow(grid, 680, 340);

            try
            {
                window.Show();
                grid.SelectedItem = rows[0];
                grid.SelectedItems.Add(rows[1]);
                grid.ScrollIntoView(rows[0]);
                window.UpdateLayout();

                Assert.AreEqual(2, grid.SelectedItems.Count);
                Assert.AreEqual(1, grid.FrozenColumnCount);
                var generatedRows = Enumerable.Range(0, rows.Count)
                    .Count(index => grid.ItemContainerGenerator.ContainerFromIndex(index) != null);
                Assert.IsGreaterThan(0, generatedRows);
                Assert.IsLessThan(100, generatedRows, "Row virtualization should avoid materializing the full data set.");

                var firstRow = grid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                Assert.IsNotNull(firstRow);
                firstRow.ApplyTemplate();
                var rowHeader = firstRow.Template.FindName("PART_RowHeader", firstRow) as DataGridRowHeader;
                var details = firstRow.Template.FindName("PART_DetailsPresenter", firstRow) as DataGridDetailsPresenter;
                Assert.IsNotNull(rowHeader);
                Assert.AreEqual(Visibility.Visible, rowHeader.Visibility);
                Assert.IsNotNull(details);
                Assert.AreEqual(Visibility.Visible, details.Visibility);
                Assert.IsGreaterThan(0d, details.ActualHeight);

                var scrollViewer = grid.Template.FindName("DG_ScrollViewer", grid) as ScrollViewer;
                Assert.IsNotNull(scrollViewer);
                scrollViewer.ApplyTemplate();
                var selectAllButton = scrollViewer.Template.FindName("PART_SelectAllButton", scrollViewer) as Button;
                Assert.IsNotNull(selectAllButton);
                Assert.AreEqual(Visibility.Visible, selectAllButton.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridSelectionVisualsAreOptIn()
        {
            var rows = new[] { new EditableRow(1, "Member") };
            var grid = CreateAdvancedDataGrid(rows, out var nameColumn);
            var window = CreateTestWindow(grid, 680, 240);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            window.Resources["ZenDataGridCellFocusVisualBorderThickness"] = new Thickness(3);
            window.Resources["ZenDataGridCellValidationBorderThickness"] = new Thickness(4);

            try
            {
                window.Show();
                grid.SelectedItem = rows[0];
                grid.ScrollIntoView(rows[0]);
                window.UpdateLayout();

                var row = grid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                Assert.IsNotNull(row);
                row.ApplyTemplate();
                var rowBorder = row.Template.FindName("RowBorder", row) as Border;
                var cellsPresenter = row.Template.FindName("PART_CellsPresenter", row) as DataGridCellsPresenter;
                Assert.IsNotNull(rowBorder);
                Assert.IsNotNull(cellsPresenter);

                var cell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
                Assert.IsNotNull(cell);
                cell.ApplyTemplate();
                grid.CurrentCell = new DataGridCellInfo(rows[0], nameColumn);
                cell.Focus();
                window.UpdateLayout();

                var cellBorder = cell.Template.FindName("CellBorder", cell) as Border;
                var stateBorder = cell.Template.FindName("StateBorder", cell) as Border;
                var cellText = FindVisualDescendant<TextBlock>(cell);
                Assert.IsNotNull(cellBorder);
                Assert.IsNotNull(stateBorder);
                Assert.IsNotNull(cellText);
                Assert.AreNotEqual(
                    Color.FromRgb(0xF1, 0xF4, 0xFA),
                    ((SolidColorBrush)rowBorder.Background).Color);
                Assert.AreEqual(new Thickness(0, 0, 0, 1), cellBorder.BorderThickness);
                Assert.AreEqual(new Thickness(), stateBorder.BorderThickness);
                Assert.AreEqual(
                    Color.FromRgb(0x1D, 0x21, 0x29),
                    ((SolidColorBrush)cellText.Foreground).Color);

                grid.IsRowSelectionHighlightEnabled = true;
                grid.IsCellFocusVisualEnabled = true;
                window.UpdateLayout();

                Assert.IsTrue(row.IsSelected);
                Assert.IsTrue(grid.IsRowSelectionHighlightEnabled);
                Assert.IsTrue(ZenDataGrid.GetIsRowSelectionHighlightEnabled(row));
                Assert.IsTrue(ZenDataGrid.GetIsCellFocusVisualEnabled(cell));
                Assert.AreEqual(new Thickness(3), grid.CellFocusVisualBorderThickness);
                Assert.AreEqual(
                    new Thickness(4),
                    ZenDataGrid.GetCellValidationBorderThickness(cell));
                Assert.AreEqual(
                    Color.FromRgb(0xF1, 0xF4, 0xFA),
                    ((SolidColorBrush)rowBorder.Background).Color);
                Assert.AreEqual(new Thickness(0, 0, 0, 1), cellBorder.BorderThickness);
                Assert.AreEqual(new Thickness(3), stateBorder.BorderThickness);
                Assert.AreEqual(new Thickness(14, 0, 14, 0), cellBorder.Padding);
                Assert.AreEqual(
                    Color.FromRgb(0x1D, 0x21, 0x29),
                    ((SolidColorBrush)cellText.Foreground).Color);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridSupportsEditingAndSorting()
        {
            var rows = Enumerable.Range(0, 20)
                .Select(index => new EditableRow(index, "成员 " + index))
                .ToList();
            var grid = CreateAdvancedDataGrid(rows, out var nameColumn);
            var window = CreateTestWindow(grid, 680, 340);

            try
            {
                window.Show();
                grid.ScrollIntoView(rows[0]);
                window.UpdateLayout();

                var firstRow = grid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                Assert.IsNotNull(firstRow);
                firstRow.ApplyTemplate();
                var cellsPresenter = firstRow.Template.FindName("PART_CellsPresenter", firstRow) as DataGridCellsPresenter;
                Assert.IsNotNull(cellsPresenter);
                var firstCell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
                Assert.IsNotNull(firstCell);
                grid.CurrentCell = new DataGridCellInfo(rows[0], nameColumn);
                firstCell.Focus();
                Assert.IsTrue(grid.BeginEdit());
                window.UpdateLayout();
                var editor = FindVisualDescendant<TextBox>(firstCell);
                Assert.IsNotNull(editor);
                editor.Text = "已编辑";
                editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Assert.AreEqual("已编辑", rows[0].Name);
                Assert.IsTrue(grid.CommitEdit(DataGridEditingUnit.Cell, true));

                var header = FindVisualDescendants<DataGridColumnHeader>(grid)
                    .FirstOrDefault(candidate => candidate.Column == nameColumn);
                Assert.IsNotNull(header);
                var onClick = typeof(ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(onClick);
                onClick.Invoke(header, null);
                window.UpdateLayout();
                Assert.AreEqual(ListSortDirection.Ascending, nameColumn.SortDirection);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
