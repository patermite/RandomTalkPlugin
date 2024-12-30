using Dalamud.Game.Text.SeStringHandling.Payloads;
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

namespace RandomTalkPlugin.CommandTracker
{
    public class Inviter
    {
        private IntPtr uiModule;
        private Int64 uiInvite;
        private IntPtr groupManagerAddress;
        private delegate char EasierProcessInviteDelegate(Int64 a1, Int64 a2, Int16 world_id, IntPtr name, char a5);
        private EasierProcessInviteDelegate _EasierProcessInvite;
        public string Name => "Inviter";
        public static ISigScanner SigScanner { get; private set; }
        public ModuleLog moduleLog { get; set; }
        private int maxDelay = 3;
        private static object LockInviteObj = new object();
        private List<string> inviteQuene = new List<string> { };
        private string currentPlayer = "";

        public void Init()
        {
            
            SigScanner = new SigScanner(); 
            var InviteToPartyByName = SigScanner.ScanText("E8 ?? ?? ?? ?? EB CC CC");
            _EasierProcessInvite = Marshal.GetDelegateForFunctionPointer<EasierProcessInviteDelegate>(InviteToPartyByName);
            var InviteToPartyInInstance = SigScanner.ScanText("E8 ?? ?? ?? ?? 4C 8B 65 ?? E9 ?? ?? ?? ?? 48 8B 83 ?? ?? ?? ??");
            nint easierProcessCIDPtr;
            try
            {
                easierProcessCIDPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 78");
            }
            catch
            {
                easierProcessCIDPtr = SigScanner.ScanText("40 53 48 83 EC 20 48 8B DA 48 8D 0D ?? ?? ?? ?? 8B 52 10 E8 ?? ?? ?? ?? 48 85 C0 74 30");
            }
            groupManagerAddress = SigScanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? 44 8B E7");
        }
        

        public void QueneInvite(PlayerPayload senderPayload, Talker talker)
        {
            inviteQuene.Add(senderPayload.PlayerName);
            unsafe
            {
                GroupManager* groupManager = (GroupManager*)groupManagerAddress;
                var group = groupManager->GetGroup();
                if (group->MemberCount >= 2)
                {
                    moduleLog.Information($"有玩家正在对话，还不能进组.");
                    talker.TalkToQuenePlayerInSayChannel(inviteQuene, senderPayload.PlayerName);
                    return;
                }
            }

            ProcessInvite(senderPayload);
            inviteQuene.Remove(senderPayload.PlayerName);
        }

        public void ProcessInvite(PlayerPayload player)
        {
            int delay = Math.Max(0, maxDelay);
            Thread.Sleep(delay);
            //pluginLog.Information($"Invite:{player.PlayerName}@{player.World.Name}");
            string player_name = player.PlayerName;
            var player_bytes = Encoding.UTF8.GetBytes(player_name);
            IntPtr mem1 = Marshal.AllocHGlobal(player_bytes.Length + 1);
            Marshal.Copy(player_bytes, 0, mem1, player_bytes.Length);
            Marshal.WriteByte(mem1, player_bytes.Length, 0);
            lock (LockInviteObj)
            {
                _EasierProcessInvite(uiInvite, 0, (short)player.World.RowId, mem1, (char)1);
            }
            Marshal.FreeHGlobal(mem1);
        }
    }
}
