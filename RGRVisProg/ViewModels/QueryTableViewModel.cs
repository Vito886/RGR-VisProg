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
    public class QueryTableViewModel : ViewModelBase
    {
        private List<List<object>> queryList;
        private List<Dictionary<string, object?>> queryDictionaries;
        public QueryTableViewModel(List<Dictionary<string, object?>> _queryDict)
        {
            queryDictionaries = _queryDict;
            queryList = new List<List<object>>();

            List<string> properties = new List<string>();

            foreach (var property in _queryDict[0])
            {
                properties.Add(property.Key);
            }

            foreach (string property in properties)
            {
                List<object> values = new List<object>();
                values.Add(property + "    ");
                values.Add(" ");
                foreach (Dictionary<string, object?> item in _queryDict)
                {
                    values.Add(item[property]);
                }
                queryList.Add(values);
            }
        }

        public List<List<object>> QueryList
        {
            get
            {
                return queryList;
            }
        }

        public override List<Dictionary<string, object?>> GetRows()
        {
            return queryDictionaries;
        }
    }
}
