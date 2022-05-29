using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Trainer
    {
        public Trainer()
        {
            DogOwnerNameNavigations = new HashSet<Dog>();
            DogTrainerNameNavigations = new HashSet<Dog>();
            Name = "None";
            BestDog = "None";
        }

        public string Name { get; set; } = null!;
        public string BestDog { get; set; } = null!;

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "TrainerName": return Name;
                    case "BestDog": return BestDog;
                }
                return null;
            }
        }

        public string Key()
        {
            return "TrainerName";
        }

        public virtual ICollection<Dog> DogOwnerNameNavigations { get; set; }
        public virtual ICollection<Dog> DogTrainerNameNavigations { get; set; }
    }
}
