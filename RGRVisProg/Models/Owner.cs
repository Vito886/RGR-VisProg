using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Dogs = new HashSet<Dog>();
        }

        public byte[] Name { get; set; } = null!;

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "Name": return Name;
                }
                return null;
            }
        }

        public virtual ICollection<Dog> Dogs { get; set; }
    }
}
