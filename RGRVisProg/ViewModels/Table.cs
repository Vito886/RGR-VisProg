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
    public class Table
    {
        private string name;
        private ViewModelBase tableView;
        public Table(string _name, bool _IsSubTable, ViewModelBase _tableView, ObservableCollection<string> _Properties)
        {
            name = _name;
            IsSubTable = _IsSubTable;
            tableView = _tableView;
            Properties = _Properties;
            TableValues = new Dictionary<string, List<object?>>();
            var a = TableView.GetTable();
            dynamic table = TableView.GetTable();
            if (table != null)
            {
                foreach (string prop in Properties)
                {
                    TableValues.Add(prop, new List<object?>() { name + ": " + prop });
                }
                for (int i = 0; i < TableValues.Count; i++)
                {
                    foreach (string prop in Properties)
                    {
                        for (int j = 0; j < table.Count; j++)
                        {
                            TableValues[prop].Add(table[j][prop]);
                        }
                    }
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public bool IsSubTable { get; }

        public ViewModelBase TableView
        {
            get
            {
                return tableView;
            }
            set
            {
                tableView = value;
            }
        }

        public Dictionary<string, List<object?>> TableValues { get; }

        public ObservableCollection<string> Properties { get; set; }
    }
}
