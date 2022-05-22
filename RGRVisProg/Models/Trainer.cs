using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Trainer
    {
        public Trainer()
        {
            Dogs = new HashSet<Dog>();
        }

        public byte[] Name { get; set; } = null!;
        public byte[]? BestDog { get; set; }

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "Name": return Name;
                    case "BestDog": return BestDog;
                }
                return null;
            }
        }

        public virtual ICollection<Dog> Dogs { get; set; }
    }
}
