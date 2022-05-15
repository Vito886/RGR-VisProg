using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using System.IO;
using System;
using System.Reactive.Linq;

namespace RGRVisProg.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase page;
        private DBViewerViewModel viewer;
        private QueryManagerViewModel queryManager;
        public ViewModelBase Page
        {
            set => this.RaiseAndSetIfChanged(ref page, value);
            get => page;
        }
        public MainWindowViewModel()
        {
            viewer = new DBViewerViewModel(); 
            queryManager = new QueryManagerViewModel();  
            Page = viewer;
        }

        public void OpenQueryManager()
        {
            Page = queryManager;
        }

        public void OpenViewer()
        {
            Page = viewer;
        }
    }
}
