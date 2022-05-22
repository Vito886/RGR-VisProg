using System;
using System.Collections.Generic;

namespace RGRVisProg.Models
{
    public partial class Result
    {
        public long RunId { get; set; }
        public byte[] DogName { get; set; } = null!;
        public long? Number { get; set; }
        public long? Position { get; set; }
        public byte[]? Time { get; set; }

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

        public virtual Run Run { get; set; } = null!;
    }
}
