using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Run
    {
        public Run()
        {
            Results = new HashSet<Result>();
            Id = 0;
            Track = "None";
            Dist = 0;
            Date = "00-None-00";
        }

        public long Id { get; set; }
        public string Track { get; set; } = null!;
        public long Dist { get; set; }
        public string Date { get; set; } = null!;

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "RunId": return Id;
                    case "Track": return Track;
                    case "Dist": return Dist;
                    case "Date": return Date;
                }
                return null;
            }
        }

        public string Key()
        {
            return "RunId";
        }

        public virtual ICollection<Result> Results { get; set; }
    }
}
