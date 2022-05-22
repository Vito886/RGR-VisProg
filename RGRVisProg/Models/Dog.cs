using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Dog
    {
        public byte[] Name { get; set; } = null!;
        public byte[]? OwnerName { get; set; }
        public byte[]? TrainerName { get; set; }
        public long? Runners { get; set; }
        public long? Starts { get; set; }
        public long? Wins { get; set; }
        public long? _2nds { get; set; }
        public long? _3rds { get; set; }
        public long? W { get; set; }
        public long? P { get; set; }

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "Name": return Name;
                    case "OwnerName": return OwnerName;
                    case "TrainerName": return TrainerName;
                    case "Runners": return Runners;
                    case "Starts": return Starts;
                    case "Wins": return Wins;
                    case "_2nds": return _2nds;
                    case "_3rds": return _3rds;
                    case "W": return W;
                    case "P": return P;
                }
                return null;
            }
        }

        public virtual Owner? OwnerNameNavigation { get; set; }
        public virtual Trainer? TrainerNameNavigation { get; set; }
    }
}
