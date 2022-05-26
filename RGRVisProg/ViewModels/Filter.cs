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
    public class Filter
    {
        public Filter(string _BoolOper, ObservableCollection<string> _Columns)
        {
            BoolOper = _BoolOper;
            Columns = _Columns;
            Operators = new ObservableCollection<string> {
                    ">", ">=", "=", "<>", "<", "<="
                };
        }
        public string BoolOper { get; set; }
        public ObservableCollection<string> Columns { get; set; }
        public ObservableCollection<string> Operators { get; set; }
        public string FilterVal { get; set; }
    }
}
