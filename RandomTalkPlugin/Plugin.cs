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
        public ICommandManager CommandManager { get; init; }
        public static RandomTalkPlugin Instance;

        private const string CommandRadnomTalkName = "/randomtalk";
        private const string CommandSetGiftOn = "/setgift";
        private const string CommandSetGiftOff = "/unsetgift";
        private const string CommandStartLotteryOn = "/startlottery";
        private const string CommandStartLotteryOff= "/unstartlottery";
        public bool SetGiftSwitch = false;
        public bool StartLotterySwitch = false;
        public Configuration Configuration { get; init; }
        public IChatGui ChatGui { get; init; }

        public readonly WindowSystem WindowSystem = new("RandomTalkPlugin");
        private LotteryWindows LotteryWindows { get; init; }
        private RadomCommandHelper CommandTracker { get; init; }
        private LotterydHelper LotteryHelper { get; init; }
        private Talker Talker { get; init; }
        public LotterydSaver LotterySaver { get; init; }
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

        public RandomTalkPlugin(IChatGui chatGui, ICommandManager commandManager)
        {
            Instance = this;
            this.ChatGui = chatGui;
            this.CommandManager = commandManager;
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            // you might normally want to embed resources and load them from the manifest stream
            var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

            CommandTracker = new RadomCommandHelper();
            LotteryHelper = new LotterydHelper();
            LotterySaver = new LotterydSaver();
            Talker = new Talker(); 
            this.PluginUI = new PluginUI(this);

            this.CommandManager.AddHandler(CommandRadnomTalkName, new CommandInfo(OnRandomTalkCommand)
            {
                HelpMessage = CommandRadnomTalkName + " can open the control window of the plugin"
            });
            this.CommandManager.AddHandler(CommandSetGiftOn, new CommandInfo(OnSetGiftCommandOn)
            {
                HelpMessage = CommandSetGiftOn + " can turn on the control of set gift function"
            });
            this.CommandManager.AddHandler(CommandSetGiftOff, new CommandInfo(OnSetGiftCommandOff)
            {
                HelpMessage = CommandSetGiftOff + " can turn off the control of set gift function "
            });
            this.CommandManager.AddHandler(CommandStartLotteryOn, new CommandInfo(OnStartLotteryCommandOn)
            {
                HelpMessage = CommandStartLotteryOn + " can turn on the control of lottery"
            });
            this.CommandManager.AddHandler(CommandStartLotteryOff, new CommandInfo(OnStartLotteryCommandOff)
            {
                HelpMessage = CommandStartLotteryOff + " can turn off the control of lottery"
            });
            this.ChatGui.ChatMessage += Chat_OnRandomDiceMessage;
            // PluginLog.Information(CommandTracker.GetRandomCommandRes());
            PluginInterface.UiBuilder.OpenMainUi += DrawLotteryUI;
            PluginInterface.UiBuilder.Draw += DrawUI;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenMainUi -= DrawLotteryUI;

            CommandManager.RemoveHandler(CommandRadnomTalkName);
            CommandManager.RemoveHandler(CommandSetGiftOn);
            CommandManager.RemoveHandler(CommandSetGiftOff);
            CommandManager.RemoveHandler(CommandStartLotteryOn);
            CommandManager.RemoveHandler(CommandStartLotteryOff);
            this.ChatGui.ChatMessage -= Chat_OnRandomDiceMessage;
            this.ChatGui.ChatMessage -= Chat_OnFreeCompanyMessage;
            this.ChatGui.ChatMessage -= Chat_OnSayGiftMessage;
        }

        private void OnRandomTalkCommand(string command, string args)
        {
            // in response to the slash command, just toggle the display status of our main ui
            this.PluginUI.RandomTalkWindow.Visible = true;
        }

        private void OnSetGiftCommandOn(string command, string args)
        {
            if (SetGiftSwitch) {
                return;
            }

            this.ChatGui.ChatMessage += Chat_OnSayGiftMessage;
            SetGiftSwitch = true;
            ChatGui.Print("Say gift set on sucess");
        }

        private void OnSetGiftCommandOff(string command, string args)
        {
            if (!SetGiftSwitch)
            {
                return;
            }
            this.ChatGui.ChatMessage -= Chat_OnSayGiftMessage;
            SetGiftSwitch = false;
            ChatGui.Print("Say gift set off sucess");
        }

        private void OnStartLotteryCommandOn(string command, string args)
        {
            if (StartLotterySwitch)
            {
                return;
            }
            this.ChatGui.ChatMessage += Chat_OnFreeCompanyMessage;
            StartLotterySwitch = true;
            ChatGui.Print("Start Lottery set on sucess");
        }
        private void OnStartLotteryCommandOff(string command, string args)
        {
            if (!StartLotterySwitch)
            {
                return;
            }
            this.ChatGui.ChatMessage -= Chat_OnFreeCompanyMessage;
            StartLotterySwitch = false;
            ChatGui.Print("Start Lottery set off sucess");
        }

        private void DrawUI()
        {
            this.PluginUI.Draw();
        }

        private void DrawLotteryUI()
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

        public void Chat_OnFreeCompanyMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
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
            Talker.TalkInLotteryRes(intNum, name, number, LotterySaver);
        }
        public void Chat_OnSayGiftMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type != XivChatType.Say) return;
            var (name, giftName) = LotteryHelper.GetGiftRes(message);
            if (name == "" || giftName == "") {
                ChatGui.Print("设置礼物失败，礼物提供者：" + name + "， 礼物名：" + giftName);
                return;
            }
            LotterySaver.SetGift(name, giftName);
            ChatGui.Print("设置礼物成功，礼物提供者：" + name + "， 礼物名：" + giftName);
        }
    }
}



