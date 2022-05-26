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
        private DBViewerViewModel DbViewer;
        private ObservableCollection<Table> tables;
        private ObservableCollection<Table> allTables;
        private ObservableCollection<Table> requests;
        private ObservableCollection<string> columnList;
        private ObservableCollection<Filter> filters;
        private ObservableCollection<Filter> groupFilters;
        private MainWindowViewModel mainWindow;
        internal Dictionary<string, string> Keys = new Dictionary<string, string>(){};

        public QueryManagerViewModel(DBViewerViewModel _DBViewer, MainWindowViewModel _mainWindow)
        {
            DbViewer = _DBViewer;
            mainWindow = _mainWindow;
            tables = DbViewer.Tables;
            allTables = DbViewer.AllTables;
            requests = new ObservableCollection<Table>();
            filters = new ObservableCollection<Filter>();
            groupFilters = new ObservableCollection<Filter>();
            columnList = new ObservableCollection<string>();

            SelectedTables = new ObservableCollection<Table>();
            SelectedColumns = new ObservableCollection<string>();

            ResultTable = new List<Dictionary<string, object?>>();
            JoinedTable = new List<Dictionary<string, object?>>();
            SelectedColumnsTable = new List<Dictionary<string, object?>>();
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
            //List<List<object>> list = new List<List<object>>();
            //for (int j = 0; j < 15; j++)
            //{
            //    List<object> a = new List<object>();
            //    for (int i = 0; i < 15; i++)
            //    {
            //        a.Add("Ok" + i.ToString());
            //    }
            //    list.Add(a);
            //}
            Requests.Add(new Table(tableName, true, new QueryTableViewModel(ResultTable.ToList()), new ObservableCollection<string>()));
            AllTables.Add(Requests.Last());
            ClearAll();
            mainWindow.OpenDBViewer();
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
                var check = SelectedTables.Where(tab => tab.Name == "Events");
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

        public List<Dictionary<string, object?>> ResultTable { get; set; }
        public List<Dictionary<string, object?>> JoinedTable { get; set; }
        public List<Dictionary<string, object?>> SelectedColumnsTable { get; set; }
        public ObservableCollection<string> ColumnList
        {
            get => columnList;
            set
            {
                this.RaiseAndSetIfChanged(ref columnList, value);
            }
        }
        public ObservableCollection<string> SelectedColumns { get; set; }
        public ObservableCollection<Filter> Filters
        {
            get => filters;
            set
            {
                this.RaiseAndSetIfChanged(ref filters, value);
            }
        }
        public ObservableCollection<Filter> GroupFilters
        {
            get => groupFilters;
            set
            {
                this.RaiseAndSetIfChanged(ref groupFilters, value);
            }
        }
        public ObservableCollection<Table> Tables
        {
            get => tables;
            set
            {
                this.RaiseAndSetIfChanged(ref tables, value);
            }
        }
        public ObservableCollection<Table> SelectedTables { get; set; }
        public ObservableCollection<Table> AllTables
        {
            get => allTables;
            set
            {
                this.RaiseAndSetIfChanged(ref allTables, value);
            }
        }
        public ObservableCollection<Table> Requests
        {
            get => requests;
            set
            {
                this.RaiseAndSetIfChanged(ref requests, value);
            }
        }
        public DBViewerViewModel DBViewer { get; }
    }
}
