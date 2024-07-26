using Dalamud.Plugin.Services;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Game.Text.SeStringHandling;
using System.Runtime.CompilerServices;
using Dalamud.Logging;
namespace RandomTalkPlugin.Lottery
{
    public class LotterydSaver
    {
        private Dictionary<int, (string, string)> giftDict = new Dictionary<int, (string,string)> { };
        private Dictionary<(string, string),  string> giftDestinationDict = new Dictionary<(string, string), string> { };
        public (string, string) GetGift(int number, string name)
        {
            if (giftDict.TryGetValue(number, out (string,string) value))
            {

                giftDestinationDict[value] = name;
                giftDict.Remove(number);
                return value;
            }
            return ("", "");
        }

        public void SetGift(string name, string giftName)
        {
            giftDict[giftDict.Count + 1] = (name, giftName);
            var shuffledNumbers = Enumerable.Range(1, giftDict.Count).OrderBy(x => Guid.NewGuid());
            var valueList = giftDict.Values.ToList();
            var tempDict = new Dictionary<int, (string, string)> { };
            for (int i = 0; i < giftDict.Count; i++)
            {
                tempDict[shuffledNumbers.ElementAt(i)] = valueList[i];
            }
            giftDict = tempDict;
            PluginLog.Information("Set Success");
        }
        public Dictionary<int, (string, string)> GetGiftGift(string name, string giftName) { return giftDict; }

        public Dictionary<(string, string), string> GetDestinationGiftGift(string name, string giftName) { return giftDestinationDict; }


    }
    
}
