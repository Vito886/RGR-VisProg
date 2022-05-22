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
    public class TrainersTableViewModel : ViewModelBase
    {
        private ObservableCollection<Trainer> table;
        public TrainersTableViewModel(ObservableCollection<Trainer> _trainers)
        {
            Table = _trainers;
        }

        public ObservableCollection<Trainer> Table
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

        public override ObservableCollection<Trainer> GetTable()
        {
            return Table;
        }
    }
}
