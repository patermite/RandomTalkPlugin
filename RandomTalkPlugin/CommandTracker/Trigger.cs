using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dalamud.Plugin.Services;

namespace RandomTalkPlugin.CommandTracker
{
    public class Trigger
    {
        public string Name { get; set; }
        public string World { get; set; }
        public long CurrentAmount { get; set; }
        public IChatGui ChatGui { get; init; }

        [NonSerialized]
        public List<RandomCommandTracker> Transactions = new List<RandomCommandTracker>();
        public Talker Talker = new Talker{ };
        internal void RandomCommandTracker(RandomCommandTracker RandomCommandTracker)
        {
            Talker.Talk();
        } 
    }
}
