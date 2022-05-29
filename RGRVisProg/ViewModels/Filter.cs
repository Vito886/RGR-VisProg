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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RGRVisProg.ViewModels
{
    public class Filter : INotifyPropertyChanged
    {
        private string? operatoor;
        private string example;
        private bool isValueInputSupported;
        public Filter(string _BoolOper, ObservableCollection<string> _Columns)
        {
            BoolOper = _BoolOper;
            Columns = _Columns;
            Operator = "";
            Column = "";
            FilterVal = "";
            Example = "";
            IsValueInputSupported = true;
            Operators = new ObservableCollection<string> {
                    ">", ">=", "=", "<>", "<", "<=",
                    "In Range", "Not In Range", "Contains", "Not Contains",
                    "Is Null", "Not Null", "Belong", "Not Belong"
                };
        }
        public string? Operator
        {
            get => operatoor;
            set
            {
                operatoor = value;

                switch (operatoor)
                {
                    case ">":
                        Example = "Number";
                        IsValueInputSupported = true;
                        break;
                    case ">=":
                        Example = "Number";
                        IsValueInputSupported = true;
                        break;
                    case "=":
                        Example = "Number || String";
                        IsValueInputSupported = true;
                        break;
                    case "<>":
                        Example = "Number || String";
                        IsValueInputSupported = true;
                        break;
                    case "<":
                        Example = "Number";
                        IsValueInputSupported = true;
                        break;
                    case "<=":
                        Example = "Number";
                        IsValueInputSupported = true;
                        break;
                    case "In Range":
                        Example = "10..40";
                        IsValueInputSupported = true;
                        break;
                    case "Not In Range":
                        Example = "10..40";
                        IsValueInputSupported = true;
                        break;
                    case "Contains":
                        Example = "Substring";
                        IsValueInputSupported = true;
                        break;
                    case "Not Contains":
                        Example = "Substring";
                        IsValueInputSupported = true;
                        break;
                    case "Is Null":
                        Example = "";
                        IsValueInputSupported = false;
                        break;
                    case "Not Null":
                        Example = "";
                        IsValueInputSupported = false;
                        break;
                    case "Belong":
                        Example = "1, 2 || str1, str2";
                        IsValueInputSupported = true;
                        break;
                    case "Not Belong":
                        Example = "1, 2 || str1, str2";
                        IsValueInputSupported = true;
                        break;
                }
            }
        }
        public string Example
        {
            get => example;
            set
            {
                example = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsValueInputSupported
        {
            get => isValueInputSupported;
            set
            {
                isValueInputSupported = value;
                NotifyPropertyChanged();
            }
        }
        public string? BoolOper { get; set; }
        public string FilterVal { get; set; }
        public string? Column { get; set; }
        public ObservableCollection<string> Columns { get; set; }
        public ObservableCollection<string> Operators { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
