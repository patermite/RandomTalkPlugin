using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;

namespace RandomTalkPlugin.CommandTracker
{
    public class Talker
    {
        public DateTime TimeStamp { get; set; }
        public String Name { get; set; }
        public long Number { get; set; }
        public long Total { get; set; }
        public IChatGui ChatGui { get; init; }

        public void Talk()
        {
        }

    }
}
