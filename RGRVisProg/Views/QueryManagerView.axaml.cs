using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using RGRVisProg.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RGRVisProg.Views
{
    public partial class QueryManagerView : UserControl
    {
        public QueryManagerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
