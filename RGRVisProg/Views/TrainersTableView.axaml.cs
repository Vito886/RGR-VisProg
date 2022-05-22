using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RGRVisProg.Views
{
    public partial class TrainersTableView : UserControl
    {
        public TrainersTableView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DeleteNullColumn(object control, DataGridAutoGeneratingColumnEventArgs args)
        {
            if (args.PropertyName == "Dogs" || args.PropertyName == "Item")
            {
                args.Cancel = true;
            }
        }
    }
}
