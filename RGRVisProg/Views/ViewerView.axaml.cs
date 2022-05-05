using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RGRVisProg.Views
{
    public partial class ViewerView : UserControl
    {
        public ViewerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
