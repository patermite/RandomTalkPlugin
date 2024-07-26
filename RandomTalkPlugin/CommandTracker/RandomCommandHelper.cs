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
namespace RandomTalkPlugin.CommandTracker
{


    public class RadomCommandHelper
    {
        [PluginService]
        private IPluginLog PluginLog { get; init; }
        [PluginService]
        private IChatGui Chat { get; init; }
        public (string, string) GetRandomCommandRes(SeString message)
        {          
            return (message.TextValue.Split("掷出了")[0], message.TextValue.Split("掷出了")[1].Split("点")[0]);
        }
    }
    
}
