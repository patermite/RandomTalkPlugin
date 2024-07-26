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
namespace RandomTalkPlugin.Lottery
{
    public class LotterydHelper
    { 
        public string GetLotteryNumberRes(SeString message)
        {          
            if (message.TextValue.Contains("我选")) {
                return message.TextValue.Split("我选")[1].Split("号")[0];
            }
            return "";
        }
    }
    
}
