using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using RandomTalkPlugin.Windows;
using RandomTalkPlugin.CommandTracker;
using RandomTalkPlugin.Lottery;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Logging;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using System;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using Dalamud.Plugin.Ipc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.String;
using System.Threading.Channels;
using System.Collections.Generic;


namespace RandomTalkPlugin
{
    public unsafe class RandomTalkPlugin : IDalamudPlugin
    {
        [PluginService] internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        

        private const string CommandName = "/randomtalk";
        private const string SetGiftCommandName = "/setgift";
        public static RandomTalkPlugin Instance;
        public Configuration Configuration { get; init; }
        public IChatGui ChatGui { get; init; }

        public readonly WindowSystem WindowSystem = new("RandomTalkPlugin");
        private LotteryWindows LotteryWindows { get; init; }
        private RadomCommandHelper CommandTracker { get; init; }
        private LotterydHelper LotteryHelper { get; init; }
        private LotterydSaver LotterySaver { get; init; }
        public PluginUI PluginUI { get; init; }
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr ParseMessageDelegate(IntPtr a, IntPtr b);
        private readonly Hook<ParseMessageDelegate> parseMessageHook;
        private delegate void MacroCallDelegate(RaptureShellModule* raptureShellModule, RaptureMacroModule.Macro* macro);
        public enum DeathRollChatTypes : ushort
        {
            // Random
            RandomRoll = 2122,
            OtherRandomRoll = 8266, 
        }

        public RandomTalkPlugin(IChatGui chatGui)
        {
          
            Instance = this;
            this.ChatGui = chatGui;
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            // you might normally want to embed resources and load them from the manifest stream
            var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

            CommandTracker = new RadomCommandHelper();
            LotteryHelper = new LotterydHelper();
            LotterySaver = new LotterydSaver();
            this.PluginUI = new PluginUI(this);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });
            this.ChatGui.ChatMessage += Chat_OnRandomDiceMessage;
            this.ChatGui.ChatMessage += Chat_OnFreeCompanyMessage;
            // PluginLog.Information(CommandTracker.GetRandomCommandRes());

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            CommandManager.RemoveHandler(CommandName);
            this.ChatGui.ChatMessage -= Chat_OnRandomDiceMessage;
            this.ChatGui.ChatMessage -= Chat_OnFreeCompanyMessage;
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just toggle the display status of our main ui
            this.PluginUI.LotteryWindows.Visible = true;
        }

        private void DrawUI()
        {
            this.PluginUI.Draw();
        }

        private void DrawConfigUI()
        {
            
            this.PluginUI.LotteryWindows.Visible = true;
        }

        private void Chat_OnRandomDiceMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            var xivChatType = (ushort)type;
            var channel = xivChatType & 0x7F;
            if (!Enum.IsDefined(typeof(DeathRollChatTypes), xivChatType) && channel != 74) return;
            var (name, number) = CommandTracker.GetRandomCommandRes(message); 
            PluginLog.Information("name is: {0}, number is {1}, senderId is {2} ",name, number, senderId.ToString());
            byte[] byteArray = System.Text.Encoding.Default.GetBytes("/em 1234567");
            var line = new  Utf8String("/s 我 是 你 爸爸");
            
            var macro = RaptureMacroModule.Instance()-> GetMacro(1, 1);
            RaptureMacroModule.Instance()->SetMacroLines(macro, 0, &line);
            RaptureShellModule.Instance()->ExecuteMacro(macro);
        }

        private void Chat_OnFreeCompanyMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type != XivChatType.FreeCompany) return;
            var name = sender.TextValue;
            PluginLog.Information("name is: {0}, message is {1}, senderId is {2} ", name, message.TextValue, senderId.ToString());
            var number = LotteryHelper.GetLotteryNumberRes(message);
            if (number == "") return;
            int intNum;
            bool success = int.TryParse(number, out intNum);
            if (!success) {
                PluginLog.Error("The Number can't not parse to int: {0}", number);
                return;
            }
            PluginLog.Information("name is: {0}, number is {1}, senderId is {2} ", name, number, senderId.ToString());
            var (giftSender, giftName) = LotterySaver.GetGift(intNum, name);
            string talkStr1, talkStr2;
            talkStr1 = "/wait 2";
            if (giftSender == "") {          
                talkStr2 = "/fc " + name + "选择的是" + number + "号，但是礼物库中没有此号码，请重新选择！";

            } else {
                talkStr2 = "/fc " +  name + "选择的是" + number + "号，他抽中来自" + giftSender + "的礼物：" + giftName + "！";
            }
            var line1 = new Utf8String(talkStr1);
            var line2 = new Utf8String(talkStr2);
            var macro = RaptureMacroModule.Instance()->GetMacro(1, 0);
            RaptureMacroModule.Instance()->SetMacroLines(macro, 0, &line1);
            RaptureMacroModule.Instance()->SetMacroLines(macro, 1, &line2);
            RaptureShellModule.Instance()->ExecuteMacro(macro);
        }
    }
}



