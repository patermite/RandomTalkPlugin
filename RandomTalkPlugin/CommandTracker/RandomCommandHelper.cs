using Dalamud.Game.Text.SeStringHandling;
using System;
namespace RandomTalkPlugin.CommandTracker
{


    public class RadomCommandHelper
    {
        public Random random = new Random();
        public (string, string) GetRandomCommandRes(SeString message)
        {
            if (message.TextValue.Contains("掷出了"))
            {
                return (message.TextValue.Split("掷出了")[0], message.TextValue.Split("掷出了")[1].Split("点")[0]);
            }
            return ("", "");
        }

        public int GetRandomResForSpecifyDice(int number) { return random.Next(number); }
    }

}
