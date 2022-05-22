using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
            if (args.PropertyName == "RunNameNavigation" || args.PropertyName == "Item")
            {
                args.Cancel = true;
            }
        }
    }
}
