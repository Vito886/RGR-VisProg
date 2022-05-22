using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Run
    {
        public Run()
        {
            Results = new HashSet<Result>();
        }

        public long Id { get; set; }
        public byte[]? Track { get; set; }
        public long? Dist { get; set; }
        public byte[]? Date { get; set; }

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "Id": return Id;
                    case "Track": return Track;
                    case "Dist": return Dist;
                    case "Date": return Date;
                }
                return null;
            }
        }

        public virtual ICollection<Result> Results { get; set; }
    }
}
