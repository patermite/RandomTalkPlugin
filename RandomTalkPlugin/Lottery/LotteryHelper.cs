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
using System.Diagnostics.Tracing;
namespace RandomTalkPlugin.Lottery
{
    public class LotterydHelper
    { 
        public string GetLotteryNumberRes(SeString message)
        {          
            if (message.TextValue.Contains("我选") && message.TextValue.Contains("号")) {
                return message.TextValue.Split("我选")[1].Split("号")[0];
            }
            return "";
        }

        public (string, string) GetGiftRes(SeString message)
        {
            string[] words = message.ToString().Split(' ');
            if (message.TextValue.Contains("加入礼物") && words.Count() >= 3)
            {
                return (words.ElementAt(1), words.ElementAt(2));
            }
            return ("", "");
        }
    }
    
}
