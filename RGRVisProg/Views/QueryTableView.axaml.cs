using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RGRVisProg.Views
{
    public partial class QueryTableView : UserControl
    {
        public QueryTableView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
