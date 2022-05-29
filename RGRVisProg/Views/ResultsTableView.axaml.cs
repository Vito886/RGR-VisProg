using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RGRVisProg.ViewModels;

namespace RGRVisProg.Views
{
    public partial class ResultsTableView : UserControl
    {
        public ResultsTableView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DeleteNullColumn(object control, DataGridAutoGeneratingColumnEventArgs args)
        {
            if (args.PropertyName == "DogNameNavigation" || args.PropertyName == "RunNameNavigation" || args.PropertyName == "Item" || args.PropertyName == "Run")
            {
                args.Cancel = true;
            }
        }

        private void RowSelected(object control, SelectionChangedEventArgs args)
        {
            DataGrid? grid = control as DataGrid;
            ViewModelBase? context = this.DataContext as ViewModelBase;
            if (grid != null && context != null)
            {
                if (context.RemoveInProgress)
                    return;
                context.RemovableItems.Clear();
                foreach (object item in grid.SelectedItems)
                {
                    context.RemovableItems.Add(item);
                }
            }
        }
    }
}
