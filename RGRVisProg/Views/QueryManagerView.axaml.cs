using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using RGRVisProg.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Avalonia.Interactivity;
using System.Linq;

namespace RGRVisProg.Views
{
    public partial class QueryManagerView : UserControl
    {
        public QueryManagerView()
        {
            InitializeComponent();
            FilterAND = this.FindControl<Button>("FilterAND");
            FilterOR = this.FindControl<Button>("FilterOR");
            FilterPop = this.FindControl<Button>("FilterPop");
            GroupFilterAND = this.FindControl<Button>("GroupFilterAND");
            GroupFilterOR = this.FindControl<Button>("GroupFilterOR");
            GroupFilterPop = this.FindControl<Button>("GroupFilterPop");
            RequestName = this.FindControl<TextBox>("RequestName");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void IsTableExist(QueryManagerViewModel context)
        {
            bool tableExist = false;
            foreach (Table table in context.AllTables)
            {
                if (table.Name == RequestName.Text)
                {
                    tableExist = true;
                    break;
                }
            }
            if (RequestName.Text != "" && RequestName.Text != null && !tableExist)
                this.FindControl<Button>("Accept").IsEnabled = true;
            else
                this.FindControl<Button>("Accept").IsEnabled = false;
        }

        private void AddRequest(object control, RoutedEventArgs args)
        {
            QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
            if (context != null)
            {
                context.AddRequest(this.FindControl<TextBox>("RequestName").Text);
                this.FindControl<Button>("Accept").IsEnabled = false;
            }
        }

        private void RequestNameChanged(object control, KeyEventArgs args)
        {
            TextBox? requestName = control as TextBox;
            if (requestName != null)
            {
                QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
                if (context != null)
                {
                    if (context.ResultTable.Count == 0)
                    {
                        this.FindControl<Button>("Accept").IsEnabled = false;
                    }
                    else
                    {
                        IsTableExist(context);
                    }
                }
            }
        }

        private void TableSelected(object control, SelectionChangedEventArgs args)
        {
            ListBox? tablesList = control as ListBox;
            if (tablesList != null)
            {
                QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
                if (context != null)
                {
                    context.SelectedTables = new ObservableCollection<Table>();
                    foreach (Table table in tablesList.SelectedItems)
                    {
                        if (!table.IsSubTable)
                        {
                            context.SelectedTables.Add(table);
                            context.IsBDTableSelected = true;
                        }
                        else
                        {
                            context.ClearAll();
                            foreach (Table subTable in tablesList.SelectedItems)
                            {
                                if (subTable.IsSubTable)
                                {
                                    context.SelectedTables.Add(subTable);
                                }
                            }
                            context.IsBDTableSelected = false;
                            break;
                        }
                    }
                    context.Join();

                    if (context.ResultTable.Count == 0)
                    {
                        this.FindControl<Button>("Accept").IsEnabled = false;
                    }
                    else
                    {
                        IsTableExist(context);
                    }
                }
            }
        }

        private void ColumnSelected(object control, SelectionChangedEventArgs args)
        {
            ListBox? tablesList = control as ListBox;
            if (tablesList != null)
            {
                QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
                if (context != null)
                {
                    context.SelectedColumns.Clear();
                    context.Filters.Clear();
                    context.GroupFilters.Clear();
                    context.Filters.Add(new Filter("", context.SelectedColumns));
                    context.GroupFilters.Add(new Filter("", context.SelectedColumns));
                    foreach (string column in tablesList.SelectedItems)
                    {
                        context.SelectedColumns.Add(column);
                    }
                    context.Select();

                    if (context.ResultTable.Count == 0)
                    {
                        this.FindControl<Button>("Accept").IsEnabled = false;
                    }
                    else
                    {
                        IsTableExist(context);
                    }

                    if (context.SelectedColumns.Count != 0)
                    {
                        FilterAND.IsEnabled = true;
                        FilterOR.IsEnabled = true;
                        FilterPop.IsEnabled = true;
                        GroupFilterAND.IsEnabled = true;
                        GroupFilterOR.IsEnabled = true;
                        GroupFilterPop.IsEnabled = true;
                    }
                    else
                    {
                        FilterAND.IsEnabled = false;
                        FilterOR.IsEnabled = false;
                        FilterPop.IsEnabled = false;
                        GroupFilterAND.IsEnabled = false;
                        GroupFilterOR.IsEnabled = false;
                        GroupFilterPop.IsEnabled = false;
                    }
                }
            }
        }

        private void GroupingColumnSelected(object control, SelectionChangedEventArgs args)
        {
            ListBox? columnList = control as ListBox;
            if (columnList != null)
            {
                QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
                if (context != null)
                {
                    context.GroupingColumn = columnList.SelectedItem as string;
                }
            }
        }

        public void AddFilterOR(object control, RoutedEventArgs args)
        {
            QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
            Button? button = control as Button;
            if (context != null && button != null)
            {
                string? type = button.CommandParameter as string;
                if (type == "Default")
                {
                    context.Filters.Add(new Filter("OR", context.SelectedColumns));
                    FilterAND.IsEnabled = false;
                    FilterPop.IsEnabled = true;
                }
                else
                {
                    context.GroupFilters.Add(new Filter("OR", context.SelectedColumns));
                    GroupFilterAND.IsEnabled = false;
                    GroupFilterPop.IsEnabled = true;
                }
            }
        }
        public void AddFilterAND(object control, RoutedEventArgs args)
        {
            QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
            Button? button = control as Button;
            if (context != null && button != null)
            {
                string? type = button.CommandParameter as string;
                if (type == "Default")
                {
                    context.Filters.Add(new Filter("AND", context.SelectedColumns));
                    FilterOR.IsEnabled = false;
                    FilterPop.IsEnabled = true;
                }
                else
                {
                    context.GroupFilters.Add(new Filter("AND", context.SelectedColumns));
                    GroupFilterOR.IsEnabled = false;
                    GroupFilterPop.IsEnabled = true;
                }
            }
        }
        public void PopBackFilter(object control, RoutedEventArgs args)
        {
            QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
            Button? button = control as Button;
            if (context != null && button != null)
            {
                string? type = button.CommandParameter as string;
                if (context.Filters.Count > 1 && type == "Default")
                    context.Filters.Remove(context.Filters.Last());
                else if (context.GroupFilters.Count > 1 && type == "Group")
                    context.GroupFilters.Remove(context.GroupFilters.Last());

                if (context.Filters.Count == 1 && type == "Default")
                {
                    FilterOR.IsEnabled = true;
                    FilterAND.IsEnabled = true;
                    FilterPop.IsEnabled = false;
                }
                else if (context.GroupFilters.Count == 1 && type == "Group")
                {
                    GroupFilterOR.IsEnabled = true;
                    GroupFilterAND.IsEnabled = true;
                    GroupFilterPop.IsEnabled = false;
                }
            }
        }

        private void BackToViewer(object control, RoutedEventArgs args)
        {
            QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
            MainWindowViewModel? parentContext = this.Parent.DataContext as MainWindowViewModel;
            if (context != null && parentContext != null)
            {
                context.ClearAll();
                parentContext.OpenDBViewer();
            }
        }

        private void ComboBoxSelectChanged(object control, SelectionChangedEventArgs args)
        {
            ComboBox? comboBox = control as ComboBox;
            if (comboBox != null)
            {
                Filter? filterContext = comboBox.DataContext as Filter;
                if (filterContext != null && comboBox.Name != null)
                {
                    if (comboBox.Name.Contains("Columns"))
                    {
                        filterContext.Column = comboBox.SelectedItem as string;
                    }
                    else if (comboBox.Name.Contains("Operators"))
                    {
                        filterContext.Operator = comboBox.SelectedItem as string;
                    }
                }
            }
        }

        private void FilterValueChanged(object control, KeyEventArgs args)
        {
            TextBox? filterValue = control as TextBox;
            if (filterValue != null)
            {
                QueryManagerViewModel? context = this.DataContext as QueryManagerViewModel;
                if (context != null)
                {
                    if (context.ResultTable.Count == 0)
                    {
                        this.FindControl<Button>("Accept").IsEnabled = false;
                    }
                    else
                    {
                        IsTableExist(context);
                    }
                }
            }
        }
    }
}
