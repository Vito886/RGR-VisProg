using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RGRVisProg.Views
{
    public partial class RunsTableView : UserControl
    {
        public RunsTableView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DeleteNullColumn(object control, DataGridAutoGeneratingColumnEventArgs args)
        {
            if (args.PropertyName == "Results" || args.PropertyName == "Item")
            {
                args.Cancel = true;
            }
        }
    }
}
