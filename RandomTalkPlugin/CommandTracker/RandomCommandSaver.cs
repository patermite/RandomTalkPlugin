using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTalkPlugin.CommandTracker
{
    public class RandomCommandTracker
    {
        public DateTime TimeStamp { get; set; }
        public String Name { get; set; }
        public long Number { get; set; }
        public long Total { get; set; }

        public override string ToString()
        {
            return $"{TimeStamp} {Name} has roll {Number.ToString()}";  
        }

        public string ToFileLine()
        {
            return $"{TimeStamp.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)};{Total};{Number}";
        }
        public static RandomCommandTracker FromFileLine(string line)
        {
            string[] parts = line.Split(";");
            return new RandomCommandTracker
            {
                TimeStamp = DateTime.ParseExact(parts[0], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
                Total = Convert.ToInt64(parts[1]),
                Number = Convert.ToInt64(parts[2])
            };
        }
    }
}
