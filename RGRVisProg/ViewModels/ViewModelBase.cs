using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RGRVisProg.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public virtual object GetTable() { return null; }
    }
}
