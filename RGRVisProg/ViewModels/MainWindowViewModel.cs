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
using System.Reactive.Linq;

namespace RGRVisProg.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase page;
        private DBViewerViewModel dbViewer;
        private QueryManagerViewModel queryManager;

        public ViewModelBase Page
        {
            set => this.RaiseAndSetIfChanged(ref page, value);
            get => page;
        }

        //{Binding $parent[Window].DataContext.OpenQueryManager}
        public MainWindowViewModel()
        {
            dbViewer = new DBViewerViewModel();
            queryManager = new QueryManagerViewModel(dbViewer, this);
            Page = dbViewer;
        }

        public void OpenQueryManager()
        {
            Page = queryManager;
            queryManager.DeleteRequests();
            //Observable.Merge().Take(1)
            //    .Subscribe((note) =>
            //    {

            //        Page = dbViewer;
            //    });
        }

        public void OpenDBViewer()
        {
            Page = dbViewer;
            //Observable.Merge().Take(1)
            //    .Subscribe((note) =>
            //    {

            //        Page = dbViewer;
            //    });
        }
    }
}
