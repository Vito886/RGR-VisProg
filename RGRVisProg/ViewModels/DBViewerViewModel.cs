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
    public class DBViewerViewModel : ViewModelBase
    {
        private ObservableCollection<Table> tables;
        private ObservableCollection<Dog> dogs;
        private ObservableCollection<Owner> owners;
        private ObservableCollection<Run> runs;
        private ObservableCollection<Result> results;
        private ObservableCollection<Trainer> trainers;
        private ObservableCollection<Table> requests;

        private ObservableCollection<string> FindProperties(string entityName, List<string> properties)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            for (int i = 0; i < properties.Count(); i++)
            {
                if (properties[i].IndexOf("EntityType:" + entityName) != -1)
                {
                    try
                    {
                        i++;
                        while (properties[i].IndexOf("(") != -1 && i < properties.Count())
                        {
                            result.Add(properties[i].Remove(properties[i].IndexOf("(")));
                            i++;
                        }
                        return result;
                    }
                    catch
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        public DBViewerViewModel()
        {
            try
            {
                tables = new ObservableCollection<Table>();
                requests = new ObservableCollection<Table>();
                var DataBase = new DogRunContext();

                string tableInfo = DataBase.Model.ToDebugString();
                tableInfo = tableInfo.Replace(" ", "");
                string[] splitTableInfo = tableInfo.Split("\r\n");
                List<string> properties = new List<string>(splitTableInfo.Where(str => str.IndexOf("Entity") != -1 ||
                                                            (str.IndexOf("(") != -1 && str.IndexOf("<") == -1) &&
                                                            str.IndexOf("Navigation") == -1 && str.IndexOf("(Owner)") == -1));



                dogs = new ObservableCollection<Dog>(DataBase.Dogs);
                tables.Add(new Table("Dogs", false, new DogsTableViewModel(dogs), FindProperties("Dog", properties)));

                owners = new ObservableCollection<Owner>(DataBase.Owners);
                tables.Add(new Table("Owners", false, new OwnersTableViewModel(owners), FindProperties("Owner", properties)));

                runs = new ObservableCollection<Run>(DataBase.Runs);
                tables.Add(new Table("Runs", false, new RunsTableViewModel(runs), FindProperties("Run", properties)));

                results = new ObservableCollection<Result>(DataBase.Results);
                tables.Add(new Table("Results", false, new ResultsTableViewModel(results), FindProperties("Result", properties)));

                trainers = new ObservableCollection<Trainer>(DataBase.Trainers);
                tables.Add(new Table("Trainers", false, new TrainersTableViewModel(trainers), FindProperties("Trainer", properties)));
            }
            catch
            {
                var a = 0;
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
        public ObservableCollection<Dog> Dogs
        {
            get => dogs;
            set
            {
                this.RaiseAndSetIfChanged(ref dogs, value);
            }
        }
        public ObservableCollection<Owner> Owners
        {
            get => owners;
            set
            {
                this.RaiseAndSetIfChanged(ref owners, value);
            }
        }
        public ObservableCollection<Run> Runs
        {
            get => runs;
            set
            {
                this.RaiseAndSetIfChanged(ref runs, value);
            }
        }
        public ObservableCollection<Result> Results
        {
            get => results;
            set
            {
                this.RaiseAndSetIfChanged(ref results, value);
            }
        }
        public ObservableCollection<Trainer> Trainers
        {
            get => trainers;
            set
            {
                this.RaiseAndSetIfChanged(ref trainers, value);
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
    }
}
