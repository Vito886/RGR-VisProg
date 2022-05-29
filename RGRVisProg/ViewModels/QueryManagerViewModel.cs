using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using RGRVisProg.Models;
using Microsoft.Data.Sqlite;
using System.IO;
using System;

namespace RGRVisProg.ViewModels
{
    public class QueryManagerViewModel : ViewModelBase
    {
        private ObservableCollection<Table> m_tables;
        private ObservableCollection<Table> m_allTables;
        private ObservableCollection<Table> m_requests;
        private ObservableCollection<string> m_columnList;
        private ObservableCollection<Filter> m_filters;
        private ObservableCollection<Filter> m_groupFilters;
        private MainWindowViewModel m_mainWindow;
        private bool isBDTableSelected;
        internal Dictionary<string, string> Keys = new Dictionary<string, string>()
        {
            { "DogName", "DogName"},
            { "OwnerName", "OwnerName"},
            { "RunId", "RunId"},
            { "TrainerName", "TrainerName"}
        };

        public QueryManagerViewModel(DBViewerViewModel _DBViewer, MainWindowViewModel _mainWindow)
        {
            DBViewer = _DBViewer;
            m_mainWindow = _mainWindow;
            m_tables = DBViewer.Tables;
            m_allTables = DBViewer.AllTables;
            m_requests = new ObservableCollection<Table>();
            m_filters = new ObservableCollection<Filter>();
            m_groupFilters = new ObservableCollection<Filter>();
            m_columnList = new ObservableCollection<string>();

            SelectedTables = new ObservableCollection<Table>();
            SelectedColumns = new ObservableCollection<string>();

            ResultTable = new List<Dictionary<string, object?>>();
            JoinedTable = new List<Dictionary<string, object?>>();
            SelectedColumnsTable = new List<Dictionary<string, object?>>();

            IsRequestSuccess = false;
            isBDTableSelected = true;
        }

        public void UpdateColumnList()
        {
            ColumnList = new ObservableCollection<string>();
            if (JoinedTable.Count != 0)
            {
                foreach (var column in JoinedTable[0])
                {
                    ColumnList.Add(column.Key);
                }
            }
            Filters.Clear();
            GroupFilters.Clear();
        }

        public void AddRequest(string tableName)
        {
            Filter("Filters");

            if (IsRequestSuccess)
            {
                ObservableCollection<string> properties = new ObservableCollection<string>();

                foreach (var item in ResultTable[0])
                {
                    properties.Add(item.Key);
                }

                Requests.Add(new Table(tableName, true, new QueryTableViewModel(ResultTable.ToList()), properties));
                AllTables.Add(Requests.Last());
            }

            IsBDTableSelected = true;

            ClearAll();

            m_mainWindow.OpenDBViewer();
        }

        public void DeleteRequests()
        {
            Requests = new ObservableCollection<Table>(Requests.Where(table => AllTables.Any(tables => tables.Name == table.Name)));
            GC.Collect();
        }

        public void ClearAll()
        {
            ResultTable.Clear();
            JoinedTable.Clear();
            SelectedColumnsTable.Clear();
            SelectedTables.Clear();
            SelectedColumns.Clear();
            Filters.Clear();
            GroupFilters.Clear();
            ColumnList.Clear();
        }

        public void Filter(string collection)
        {
            ObservableCollection<Filter> filters;
            List<Dictionary<string, object?>> result = new List<Dictionary<string, object?>>();

            if (collection == "Filters")
                filters = Filters;
            else
                filters = GroupFilters;

            bool SwitchForChain(Filter filter)
            {
                try
                {
                    switch (filter.Operator)
                    {
                        case ">":
                            {
                                ResultTable = ResultTable.Where(item =>
                                double.Parse(item[filter.Column].ToString()) > double.Parse(filter.FilterVal)
                                ).ToList();
                                return true;
                            }
                        case "<":
                            {
                                ResultTable = ResultTable.Where(item =>
                                double.Parse(item[filter.Column].ToString()) < double.Parse(filter.FilterVal)
                                ).ToList();
                                return true;
                            }
                        case "=":
                            {
                                ResultTable = ResultTable.Where(item =>
                                {
                                    var a = item[filter.Column].ToString();
                                    var b = filter.FilterVal;
                                    return item[filter.Column].ToString() == filter.FilterVal;
                                }
                                ).ToList();
                                return true;
                            }
                        case ">=":
                            {
                                ResultTable = ResultTable.Where(item =>
                                double.Parse(item[filter.Column].ToString()) >= double.Parse(filter.FilterVal)
                                ).ToList();
                                return true;
                            }
                        case "<>":
                            {
                                ResultTable = ResultTable.Where(item =>
                                item[filter.Column].ToString() != filter.FilterVal
                                ).ToList();
                                return true;
                            }
                        case "<=":
                            {
                                ResultTable = ResultTable.Where(item =>
                                double.Parse(item[filter.Column].ToString()) <= double.Parse(filter.FilterVal)
                                ).ToList();
                                return true;
                            }
                        case "In Range":
                            {
                                string separator = "..";
                                string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (range.Count() != 2)
                                    return false;

                                ResultTable = ResultTable.Where(item =>
                                {
                                    double number = double.Parse(item[filter.Column].ToString());
                                    if (number >= double.Parse(range[0]) && number <= double.Parse(range[1]))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Not In Range":
                            {
                                string separator = "..";
                                string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (range.Count() != 2)
                                    return false;

                                ResultTable = ResultTable.Where(item =>
                                {
                                    double number = double.Parse(item[filter.Column].ToString());
                                    if (number < double.Parse(range[0]) || number > double.Parse(range[1]))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Contains":
                            {
                                ResultTable = ResultTable.Where(item =>
                                {
                                    string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                    string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                    if (value.IndexOf(filterVal) != -1)
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Not Contains":
                            {
                                ResultTable = ResultTable.Where(item =>
                                {
                                    string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                    string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                    if (value.IndexOf(filterVal) == -1)
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Is Null":
                            {
                                ResultTable = ResultTable.Where(item =>
                                {
                                    string value = item[filter.Column].ToString();
                                    if (value == "None" || value == "0" || value == "0000-00-00 00:00:00"
                                       || value == "00:00" || value.Replace(" ", "") == "" || value.ToUpper() == "NULL"
                                      )
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Not Null":
                            {
                                ResultTable = ResultTable.Where(item =>
                                {
                                    string value = item[filter.Column].ToString();
                                    if (value != "None" && value != "0" && value != "0000-00-00 00:00:00"
                                       && value != "00:00" && value.Replace(" ", "") != "" && value.ToUpper() != "NULL"
                                      )
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Belong":
                            {
                                string separator = ",";
                                string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (set.Count() == 0)
                                    return false;

                                for (int i = 0; i < set.Count(); i++)
                                {
                                    set[i] = set[i].ToUpper().Replace(" ", "");
                                }

                                ResultTable = ResultTable.Where(item =>
                                {
                                    if (set.Contains(item[filter.Column].ToString().ToUpper()))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        case "Not Belong":
                            {
                                string separator = ",";
                                string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (set.Count() == 0)
                                    return false;

                                for (int i = 0; i < set.Count(); i++)
                                {
                                    set[i] = set[i].ToUpper().Replace(" ", "");
                                }

                                ResultTable = ResultTable.Where(item =>
                                {
                                    if (!set.Contains(item[filter.Column].ToString().ToUpper()))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList();
                                return true;
                            }
                        default:
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            bool SwitchForUnion(Filter filter)
            {
                try
                {
                    switch (filter.Operator)
                    {
                        case ">":
                            {
                                result = result.Union(result.Where(item =>
                                double.Parse(item[filter.Column].ToString()) > double.Parse(filter.FilterVal)
                                ).ToList()).ToList();
                                return true;
                            }
                        case "<":
                            {
                                result = result.Union(result.Where(item =>
                                double.Parse(item[filter.Column].ToString()) < double.Parse(filter.FilterVal)
                                ).ToList()).ToList();
                                return true;
                            }
                        case "=":
                            {
                                result = result.Union(result.Where(item =>
                                item[filter.Column].ToString() == filter.FilterVal
                                ).ToList()).ToList();
                                return true;
                            }
                        case ">=":
                            {
                                result = result.Union(result.Where(item =>
                                double.Parse(item[filter.Column].ToString()) >= double.Parse(filter.FilterVal)
                                ).ToList()).ToList();
                                return true;
                            }
                        case "<>":
                            {
                                result = result.Union(result.Where(item =>
                                item[filter.Column].ToString() != filter.FilterVal
                                ).ToList()).ToList();
                                return true;
                            }
                        case "<=":
                            {
                                result = result.Union(result.Where(item =>
                                double.Parse(item[filter.Column].ToString()) <= double.Parse(filter.FilterVal)
                                ).ToList()).ToList();
                                return true;
                            }
                        case "In Range":
                            {
                                string separator = "..";
                                string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (range.Count() != 2)
                                    return false;

                                result = result.Union(result.Where(item =>
                                {
                                    double number = double.Parse(item[filter.Column].ToString());
                                    if (number >= double.Parse(range[0]) && number <= double.Parse(range[1]))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Not In Range":
                            {
                                string separator = "..";
                                string[] range = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (range.Count() != 2)
                                    return false;

                                result = result.Union(result.Where(item =>
                                {
                                    double number = double.Parse(item[filter.Column].ToString());
                                    if (number < double.Parse(range[0]) || number > double.Parse(range[1]))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Contains":
                            {
                                result = result.Union(result.Where(item =>
                                {
                                    string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                    string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                    if (value.IndexOf(filterVal) != -1)
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Not Contains":
                            {
                                result = result.Union(result.Where(item =>
                                {
                                    string value = item[filter.Column].ToString().ToUpper().Replace(" ", ""); ;
                                    string filterVal = filter.FilterVal.ToUpper().Replace(" ", "");
                                    if (value.IndexOf(filterVal) == -1)
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Is Null":
                            {
                                result = result.Union(result.Where(item =>
                                {
                                    string value = item[filter.Column].ToString();
                                    if (value == "None" || value == "0" || value == "0000-00-00 00:00:00"
                                       || value == "00:00" || value.Replace(" ", "") == "" || value.ToUpper() == "NULL"
                                      )
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Not Null":
                            {
                                result = result.Union(result.Where(item =>
                                {
                                    string value = item[filter.Column].ToString();
                                    if (value != "None" && value != "0" && value != "0000-00-00 00:00:00"
                                       && value != "00:00" && value.Replace(" ", "") != "" && value.ToUpper() != "NULL"
                                      )
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Belong":
                            {
                                string separator = ",";
                                string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (set.Count() == 0)
                                    return false;

                                for (int i = 0; i < set.Count(); i++)
                                {
                                    set[i] = set[i].ToUpper().Replace(" ", "");
                                }

                                result = result.Union(result.Where(item =>
                                {
                                    if (set.Contains(item[filter.Column].ToString().ToUpper()))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        case "Not Belong":
                            {
                                string separator = ",";
                                string[] set = filter.FilterVal.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                if (set.Count() == 0)
                                    return false;

                                for (int i = 0; i < set.Count(); i++)
                                {
                                    set[i] = set[i].ToUpper().Replace(" ", "");
                                }

                                result = result.Union(result.Where(item =>
                                {
                                    if (!set.Contains(item[filter.Column].ToString().ToUpper()))
                                        return true;
                                    else
                                        return false;
                                }
                                ).ToList()).ToList();
                                return true;
                            }
                        default:
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            void ResultByUnion()
            {
                foreach (Filter filter in filters)
                {
                    if (filter.FilterVal.Replace(" ", "") == "" && filter.Operator != "Is Null" && filter.Operator != "Not Null")
                    {
                        IsRequestSuccess = false;
                        return;
                    }
                    else
                    {
                        if (!SwitchForUnion(filter))
                        {
                            IsRequestSuccess = false;
                            return;
                        }
                        else
                        {
                            IsRequestSuccess = true;
                        }
                    }
                }
                ResultTable = result;
            }

            void ResultByChain()
            {
                if (filters.Count == 1 && filters[0].FilterVal == "" && filters[0].Operator == "" && filters[0].Column == "")
                {
                    IsRequestSuccess = true;
                    return;
                }

                foreach (Filter filter in filters)
                {
                    if (filter.FilterVal.Replace(" ", "") == "" && filter.Operator != "Is Null" && filter.Operator != "Not Null")
                    {
                        IsRequestSuccess = false;
                        return;
                    }
                    else
                    {
                        if (!SwitchForChain(filter))
                        {
                            IsRequestSuccess = false;
                            return;
                        }
                        else
                        {
                            IsRequestSuccess = true;
                        }
                    }
                }
            }

            if (filters.Count != 0)
            {
                if (filters.Count > 1)
                {
                    if (filters[1].BoolOper == "AND")
                        ResultByChain();
                    else
                        ResultByUnion();
                }
                else
                {
                    ResultByChain();
                }

                if (ResultTable.Count == 0)
                {
                    IsRequestSuccess = false;
                    return;
                }
                if (IsRequestSuccess != false)
                {
                    if (collection == "GroupFilters")
                        return;
                    else
                        Group();
                }
            }
            else if (SelectedColumns.Count != 0)
            {
                if (collection == "GroupFilters")
                    return;
                else
                    Group();
            }
            else
                IsRequestSuccess = true;
        }

        public void Group()
        {
            if (GroupingColumn != null)
            {
                try
                {
                    var result = ResultTable.GroupBy(item => item[GroupingColumn]).ToList();
                    ResultTable.Clear();
                    foreach (var group in result)
                    {
                        foreach (Dictionary<string, object?> row in group)
                        {
                            ResultTable.Add(row);
                        }
                    }

                    if (ResultTable.Count != 0)
                    {
                        IsRequestSuccess = true;
                        Filter("GroupFilters");
                    }
                    else
                    {
                        IsRequestSuccess = false;
                        return;
                    }
                }
                catch
                {
                    IsRequestSuccess = false;
                    return;
                }
            }
            else if (GroupFilters.Count == 1 && GroupFilters[0].FilterVal == ""
                    && GroupFilters[0].Operator == "" && GroupFilters[0].Column == "")
            {
                IsRequestSuccess = true;
                return;
            }
            else
            {
                IsRequestSuccess = false;
                return;
            }
        }

        private bool TryJoin(string key1, List<Dictionary<string, object?>> table2, string key2)
        {
            try
            {
                JoinedTable = JoinedTable.Join(
                    table2,
                    firstItem => firstItem[key1],
                    secondItem => secondItem[key2],
                    (firstItem, secondItem) =>
                    {
                        Dictionary<string, object?> resultItem = new Dictionary<string, object?>();
                        foreach (var item in firstItem)
                        {
                            resultItem.TryAdd(item.Key, item.Value);
                        }
                        foreach (var item in secondItem)
                        {
                            if (item.Key != key2)
                                resultItem.TryAdd(item.Key, item.Value);
                        }
                        return resultItem;
                    }
                    ).ToList();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Join()
        {
            if (SelectedTables.Count > 0)
            {
                var check = SelectedTables.Where(tab => tab.Name == "Runs");
                if (check.Count() != 0)
                {
                    Table tmp = check.Last();
                    SelectedTables.Remove(check.Last());
                    SelectedTables.Add(tmp);
                }
                JoinedTable = new List<Dictionary<string, object?>>(SelectedTables[0].Rows);

                if (SelectedTables.Count > 1)
                {
                    List<Dictionary<string, object?>> joiningTable;

                    bool success = false;
                    for (int i = 1; i < SelectedTables.Count; i++)
                    {
                        joiningTable = SelectedTables[i].Rows;

                        foreach (var keysPair in Keys)
                        {
                            success = TryJoin(keysPair.Key, joiningTable, keysPair.Value);
                            if (success)
                                break;
                            else
                            {
                                success = TryJoin(keysPair.Value, joiningTable, keysPair.Key);
                                if (success)
                                    break;
                            }
                        }
                        if (!success)
                        {
                            JoinedTable.Clear();
                            ResultTable = JoinedTable;
                            UpdateColumnList();
                            return;
                        }
                    }
                }
                UpdateColumnList();
                ResultTable = JoinedTable;
            }
            else
            {
                JoinedTable.Clear();
                ResultTable = JoinedTable;
                ColumnList.Clear();
                IsBDTableSelected = true;
            }
        }

        public void Select()
        {
            SelectedColumnsTable = JoinedTable.Select(item =>
            {
                return new Dictionary<string, object?>(item.Where(property => SelectedColumns.Any(column => column == property.Key)));
            }).ToList();
            ResultTable = SelectedColumnsTable;
        }

        public bool IsRequestSuccess { get; set; }
        public bool IsBDTableSelected
        {
            get => isBDTableSelected;
            set
            {
                this.RaiseAndSetIfChanged(ref isBDTableSelected, value);
            }
        }
        public string? GroupingColumn { get; set; } = null;
        public List<Dictionary<string, object?>> ResultTable { get; set; }
        public List<Dictionary<string, object?>> JoinedTable { get; set; }
        public List<Dictionary<string, object?>> SelectedColumnsTable { get; set; }
        public ObservableCollection<string> ColumnList
        {
            get => m_columnList;
            set
            {
                this.RaiseAndSetIfChanged(ref m_columnList, value);
            }
        }
        public ObservableCollection<string> SelectedColumns { get; set; }
        public ObservableCollection<Filter> Filters
        {
            get => m_filters;
            set
            {
                this.RaiseAndSetIfChanged(ref m_filters, value);
            }
        }
        public ObservableCollection<Filter> GroupFilters
        {
            get => m_groupFilters;
            set
            {
                this.RaiseAndSetIfChanged(ref m_groupFilters, value);
            }
        }
        public ObservableCollection<Table> Tables
        {
            get => m_tables;
            set
            {
                this.RaiseAndSetIfChanged(ref m_tables, value);
            }
        }
        public ObservableCollection<Table> SelectedTables { get; set; }
        public ObservableCollection<Table> AllTables
        {
            get => m_allTables;
            set
            {
                this.RaiseAndSetIfChanged(ref m_allTables, value);
            }
        }
        public ObservableCollection<Table> Requests
        {
            get => m_requests;
            set
            {
                this.RaiseAndSetIfChanged(ref m_requests, value);
            }
        }
        public DBViewerViewModel DBViewer { get; }
    }
}
