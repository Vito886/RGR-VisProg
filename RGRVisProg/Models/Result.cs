using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Result
    {
        public Result()
        {
            RunId = 0;
            DogName = "None";
            Number = 0;
            Position = 0;
            Time = "00.00";
        }
        public long RunId { get; set; }
        public string DogName { get; set; } = null!;
        public long Number { get; set; }
        public long Position { get; set; }
        public string Time { get; set; } = null!;

        public object? this[string property]
        {
            get
            {
                switch (property)
                {
                    case "RunId": return RunId;
                    case "DogName": return DogName;
                    case "Number": return Number;
                    case "Position": return Position;
                    case "Time": return Time;
                }
                return null;
            }
        }

        public string Key()
        {
            return "RunId";
        }

        public virtual Dog DogNameNavigation { get; set; } = null!;
        public virtual Run Run { get; set; } = null!;
    }
}
