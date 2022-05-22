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
    public class RunsTableViewModel : ViewModelBase
    {
        private ObservableCollection<Run> table;
        public RunsTableViewModel(ObservableCollection<Run> _runs)
        {
            Table = _runs;
        }

        public ObservableCollection<Run> Table
        {
            get
            {
                return table;
            }
            set
            {
                table = value;
            }
        }

        public override ObservableCollection<Run> GetTable()
        {
            return Table;
        }
    }
}
