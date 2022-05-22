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
    public class OwnersTableViewModel : ViewModelBase
    {
        private ObservableCollection<Owner> table;
        public OwnersTableViewModel(ObservableCollection<Owner> _owners)
        {
            Table = _owners;
        }

        public ObservableCollection<Owner> Table
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

        public override ObservableCollection<Owner> GetTable()
        {
            return Table;
        }
    }
}
