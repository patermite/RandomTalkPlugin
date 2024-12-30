using Dalamud.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using RandomTalkPlugin.CommandTracker;
using System.Xml.Linq;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using Dalamud.Game;
using Dalamud.Logging.Internal;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace RandomTalkPlugin.CommandTracker
{
    public class Trader
    {
        public InventoryManager inventoryManager;

        public void Init()
        {
            inventoryManager = new InventoryManager();
            inventoryManager.GetInventoryContainer
           
        }
        
    }
}
