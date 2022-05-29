using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Name = "None";
        }
        public string Name { get; set; } = null!;

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "OwnerName": return Name;
                }
                return null;
            }
        }

        public string Key()
        {
            return "OwnerName";
        }
    }
}
