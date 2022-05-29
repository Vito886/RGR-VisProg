using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Dog
    {
        public Dog()
        {
            Results = new HashSet<Result>();
            Name = "None";
            OwnerName = "None";
            TrainerName = "None";
            Runners = 0;
            Starts = 0;
            Wins = 0;
            Seconds = 0;
            Thirds = 0;
            W = 0;
            P = 0;
        }

        public string Name { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string TrainerName { get; set; } = null!;
        public long Runners { get; set; }
        public long Starts { get; set; }
        public long Wins { get; set; }
        public long Seconds { get; set; }
        public long Thirds { get; set; }
        public long W { get; set; }
        public long P { get; set; }

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "DogName": return Name;
                    case "OwnerName": return OwnerName;
                    case "TrainerName": return TrainerName;
                    case "Runners": return Runners;
                    case "Starts": return Starts;
                    case "Wins": return Wins;
                    case "Seconds": return Seconds;
                    case "Thirds": return Thirds;
                    case "W": return W;
                    case "P": return P;
                }
                return null;
            }
        }

        public string Key()
        {
            return "DogName";
        }

        public virtual Trainer OwnerNameNavigation { get; set; } = null!;
        public virtual Trainer TrainerNameNavigation { get; set; } = null!;
        public virtual ICollection<Result> Results { get; set; }
    }
}
