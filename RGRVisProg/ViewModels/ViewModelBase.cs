using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RGRVisProg.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public bool RemoveInProgress { get; set; } = false;
        public List<object>? RemovableItems { get; set; }

        public virtual object? GetTable() { return null; }

        public virtual List<Dictionary<string, object?>> GetRows()
        {
            return new List<Dictionary<string, object?>>();
        }
    }
}
