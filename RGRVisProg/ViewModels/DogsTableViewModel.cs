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
    public class DogsTableViewModel : ViewModelBase
    {
        private ObservableCollection<Dog> table;
        public DogsTableViewModel(ObservableCollection<Dog> _dogs)
        {
            Table = _dogs;
        }

        public ObservableCollection<Dog> Table
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

        public override ObservableCollection<Dog> GetTable()
        {
            return Table;
        }
    }
}
