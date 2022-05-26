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
    public class GroupHandler : Handler
    {
        public GroupHandler(QueryManagerViewModel _QueryManager)
        {
            QM = _QueryManager;
        }

        public override void Try()
        {

        }
    }
}
