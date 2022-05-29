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
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;

namespace RGRVisProg.ViewModels
{
    public class DBViewerViewModel : ViewModelBase
    {
        private ObservableCollection<Table> tables;
        private ObservableCollection<Table> allTables;
        private ObservableCollection<Dog> dogs;
        private ObservableCollection<Owner> owners;
        private ObservableCollection<Run> runs;
        private ObservableCollection<Result> results;
        private ObservableCollection<Trainer> trainers;
        private bool currentTableIsSubtable;

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
                            string property = properties[i].Remove(properties[i].IndexOf("("));
                            if (entityName == "Trainer" && property == "Name")
                                result.Add("TrainerName");
                            else if (entityName == "Run" && property == "Id")
                                result.Add("RunId");
                            else if (entityName == "Dog" && property == "Name")
                                result.Add("DogName");
                            else if (entityName == "Owner" && property == "Name")
                                result.Add("OwnerName");
                            else
                                result.Add(property);

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
                DataBase = new DogRunContext();

                string tableInfo = DataBase.Model.ToDebugString();
                tableInfo = tableInfo.Replace(" ", "");
                string[] splitTableInfo = tableInfo.Split("\r\n");

                List<string> properties = new List<string>(splitTableInfo.Where(str => str.IndexOf("Entity") != -1 ||
                                                            (str.IndexOf("(") != -1 && str.IndexOf("<") == -1) &&
                                                            str.IndexOf("Navigation") == -1 && str.IndexOf("(Owner)") == -1
                                                             && str.IndexOf("Run(Run)ToPrincipalRunInverse:Results") == -1));


                DataBase.Dogs.Load<Dog>();
                Dogs = DataBase.Dogs.Local.ToObservableCollection();
                tables.Add(new Table("Dogs", false, new DogsTableViewModel(Dogs), FindProperties("Dog", properties)));

                DataBase.Owners.Load<Owner>();
                Owners = DataBase.Owners.Local.ToObservableCollection();
                tables.Add(new Table("Owners", false, new OwnersTableViewModel(Owners), FindProperties("Owner", properties)));

                DataBase.Runs.Load<Run>();
                Runs = DataBase.Runs.Local.ToObservableCollection();
                tables.Add(new Table("Runs", false, new RunsTableViewModel(Runs), FindProperties("Run", properties)));

                DataBase.Results.Load<Result>();
                Results = DataBase.Results.Local.ToObservableCollection();
                tables.Add(new Table("Results", false, new ResultsTableViewModel(Results), FindProperties("Result", properties)));

                DataBase.Trainers.Load<Trainer>();
                Trainers = DataBase.Trainers.Local.ToObservableCollection();
                tables.Add(new Table("Trainers", false, new TrainersTableViewModel(Trainers), FindProperties("Trainer", properties)));

                AllTables = new ObservableCollection<Table>(Tables.ToList());

                CurrentTableName = "Dogs";
                CurrentTableIsSubtable = false;
            }
            catch
            {

            }
        }

        public bool CurrentTableIsSubtable
        {
            get => !currentTableIsSubtable;
            set => this.RaiseAndSetIfChanged(ref currentTableIsSubtable, value);
        }
        public string CurrentTableName { get; set; }
        public DogRunContext DataBase { get; set; }
        public ObservableCollection<Table> Tables
        {
            get => tables;
            set
            {
                this.RaiseAndSetIfChanged(ref tables, value);
            }
        }
        public ObservableCollection<Table> AllTables
        {
            get => allTables;
            set
            {
                this.RaiseAndSetIfChanged(ref allTables, value);
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

        public void AddItem()
        {
            switch (CurrentTableName)
            {
                case "Dogs":
                    {
                        Dogs.Add(new Dog());
                        break;
                    }
                case "Owners":
                    {
                        Owners.Add(new Owner());
                        break;
                    }
                case "Results":
                    {
                        Results.Add(new Result());
                        break;
                    }
                case "Runs":
                    {
                        Runs.Add(new Run());
                        break;
                    }
                case "Trainers":
                    {
                        Trainers.Add(new Trainer());
                        break;
                    }
            }
        }

        public void DeleteItems()
        {
            Table currentTable = Tables.Where(table => table.Name == CurrentTableName).ToList()[0];
            List<object>? RemovableItems = currentTable.GetRemovableItems();
            currentTable.SetRemoveInProgress(true);

            if (RemovableItems != null && RemovableItems.Count != 0)
            {

                switch (CurrentTableName)
                {
                    case "Dogs":
                        {
                            for (int i = 0; i < RemovableItems.Count; i++)
                            {
                                Dogs.Remove(RemovableItems[i] as Dog);
                            }
                            break;
                        }
                    case "Owners":
                        {
                            for (int i = 0; i < RemovableItems.Count; i++)
                            {
                                Owners.Remove(RemovableItems[i] as Owner);
                            }
                            break;
                        }
                    case "Results":
                        {
                            for (int i = 0; i < RemovableItems.Count; i++)
                            {
                                Results.Remove(RemovableItems[i] as Result);
                            }
                            break;
                        }
                    case "Runs":
                        {
                            for (int i = 0; i < RemovableItems.Count; i++)
                            {
                                Runs.Remove(RemovableItems[i] as Run);
                            }
                            break;
                        }
                    case "Trainers":
                        {
                            for (int i = 0; i < RemovableItems.Count; i++)
                            {
                                Trainers.Remove(RemovableItems[i] as Trainer);
                            }
                            break;
                        }
                }
            }
            currentTable.SetRemoveInProgress(false);
        }

        public void Save()
        {
            DataBase.SaveChanges();
        }
    }
}
