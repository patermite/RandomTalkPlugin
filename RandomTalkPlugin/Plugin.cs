using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using RandomTalkPlugin.Windows;
using RandomTalkPlugin.CommandTracker;
using RandomTalkPlugin.Lottery;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging.Internal;
using System;
using System.Threading;
using System.Linq;
using Dalamud.Game.Text.SeStringHandling.Payloads;


namespace RandomTalkPlugin
{
    public unsafe class RandomTalkPlugin : IDalamudPlugin
    {
        [PluginService] 
        public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
        [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
        public ICommandManager CommandManager { get; init; }
        public static RandomTalkPlugin Instance;

        private const string CommandRadnomTalkName = "/randomtalk";
        private const string CommandLotteryTalkName = "/lotterytalk";
        private const string CommandStartLotteryOn = "/startlottery";
        private const string CommandStartLotteryOff= "/unstartlottery";
        private const string CommandStartCharacterTalk = "/startTalk";
        private const string CommandStartCharacterTalkOff = "/unstartTalk";
        public bool StartLotterySwitch = false;
        public bool StartCharacterTalkSwitch = false;
        public Configuration Configuration { get; init; }
        public IChatGui ChatGui { get; init; }
        public ModuleLog moduleLog { get; set; }

        public readonly WindowSystem WindowSystem = new("RandomTalkPlugin");
        private LotteryWindows LotteryWindows { get; init; }
        private RadomCommandHelper CommandTracker { get; init; }
        private RandomCommandSaver RandomCommandSaver { get; init; }
        private LotterydHelper LotteryHelper { get; init; }
        private Talker Talker { get; init; }
        public LotterydSaver LotterySaver { get; init; }
        public PluginUI PluginUI { get; init; }
        public PlayerAttribute PlayerAttributes { get; init; }
        public Inviter inviter { get; init; }
        public enum RollChatTypes : ushort
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
            CommandTracker = new RadomCommandHelper();
            LotteryHelper = new LotterydHelper();
            LotterySaver = new LotterydSaver();
            moduleLog = new ModuleLog("RandomTalkPlugin");
            RandomCommandSaver = new RandomCommandSaver();
            PlayerAttributes = new PlayerAttribute();
            inviter = new Inviter();
            LotterySaver.Init(PluginInterface);
            inviter.Init();
            RandomCommandSaver.LoadCharacterDialogue(PluginInterface);
            RandomCommandSaver.CheckCharacterDialogue();
            PlayerAttributes.LoadPlayerJob(PluginInterface);
            Talker = new Talker(); 
            this.PluginUI = new PluginUI(this);

            this.CommandManager.AddHandler(CommandLotteryTalkName, new CommandInfo(OnLotteryTalkCommand)
            {
                HelpMessage = CommandLotteryTalkName + " can open the control window of the lottery plugin"
            });
            this.CommandManager.AddHandler(CommandRadnomTalkName, new CommandInfo(OnRandomTalkCommand)
            {
                HelpMessage = CommandRadnomTalkName + " can open the control window of the randomtalk plugin"
            });
            this.CommandManager.AddHandler(CommandStartLotteryOn, new CommandInfo(OnStartLotteryCommandOn)
            {
                HelpMessage = CommandStartLotteryOn + " can turn on the control of lottery"
            });
            this.CommandManager.AddHandler(CommandStartLotteryOff, new CommandInfo(OnStartLotteryCommandOff)
            {
                HelpMessage = CommandStartLotteryOff + " can turn off the control of lottery"
            });
            this.CommandManager.AddHandler(CommandStartCharacterTalk, new CommandInfo(OnStartCharacterTalkCommandOn)
            {
                HelpMessage = CommandStartCharacterTalk + " can turn on the control of character talk"
            });
            this.CommandManager.AddHandler(CommandStartCharacterTalkOff, new CommandInfo(OnStartCharacterTalkCommandOff)
            {
                HelpMessage = CommandStartCharacterTalkOff + " can turn off the control of character talk"
            });
            PluginInterface.UiBuilder.OpenMainUi += DrawRandomTalkUI;
            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawLotteryUI;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenMainUi -= DrawLotteryUI;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawRandomTalkUI;

            CommandManager.RemoveHandler(CommandRadnomTalkName);
            CommandManager.RemoveHandler(CommandStartLotteryOn);
            CommandManager.RemoveHandler(CommandStartLotteryOff);
            this.ChatGui.ChatMessage -= Chat_OnRandomDiceMessage;
            this.ChatGui.ChatMessage -= Chat_OnFreeCompanyMessage;
            this.ChatGui.ChatMessage -= Chat_OnPartyMessage;
            this.ChatGui.ChatMessage -= Chat_OnSayMessageForInvite;
        }

        private void OnRandomTalkCommand(string command, string args)
        {
            // in response to the slash command, just toggle the display status of our main ui
            this.PluginUI.RandomTalkWindow.Visible = true;
        }

        private void OnLotteryTalkCommand(string command, string args)
        {
            // in response to the slash command, just toggle the display status of our main ui
            this.PluginUI.LotteryWindows.Visible = true;
        }

        private void OnStartLotteryCommandOn(string command, string args)
        {
            if (StartLotterySwitch) return;
            this.ChatGui.ChatMessage += Chat_OnFreeCompanyMessage;
            StartLotterySwitch = true;
            ChatGui.Print("Start Lottery set on sucess");
        }
        private void OnStartLotteryCommandOff(string command, string args)
        {
            if (!StartLotterySwitch) return;
            this.ChatGui.ChatMessage -= Chat_OnFreeCompanyMessage;
            StartLotterySwitch = false;
            ChatGui.Print("Start Lottery set off sucess");
        }
        private void OnStartCharacterTalkCommandOn(string command, string args)
        {
            if (StartCharacterTalkSwitch) return;
            if (!RandomCommandSaver.CheckCharacterDialogue()) return;
            this.ChatGui.ChatMessage += Chat_OnPartyMessage;
            this.ChatGui.ChatMessage += Chat_OnRandomDiceMessage;
            this.ChatGui.ChatMessage += Chat_OnSayMessageForInvite;
            StartCharacterTalkSwitch = true;
            ChatGui.Print("Start Character Talk set on sucess");
        }

        private void OnStartCharacterTalkCommandOff(string command, string args)
        {
            if (!StartCharacterTalkSwitch) return;
            this.ChatGui.ChatMessage -= Chat_OnPartyMessage;
            this.ChatGui.ChatMessage -= Chat_OnRandomDiceMessage;
            this.ChatGui.ChatMessage -= Chat_OnSayMessageForInvite;
            StartCharacterTalkSwitch = false;
            ChatGui.Print("Start Character Talk set off sucess");
        }

        private void DrawUI()
        {
            this.PluginUI.Draw();
        }

        private void DrawLotteryUI()
        {
            
            this.PluginUI.LotteryWindows.Visible = true;
        }

        private void DrawRandomTalkUI()
        {

            this.PluginUI.RandomTalkWindow.Visible = true;
        }

        public void Chat_OnRandomDiceMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            var xivChatType = (ushort)type;
            var channel = xivChatType & 0x7F;
            if (!Enum.IsDefined(typeof(RollChatTypes), xivChatType) && channel != 74) return;
            var (name, number) = CommandTracker.GetRandomCommandRes(message);
            var playerState = RandomCommandSaver.GetPlayerState(name);
            if (playerState == "")
            {
                playerState = "0";
            }
            moduleLog.Information("[RandomDice] name is: {0}, state is {1}, number is {2} ", name, playerState, number);

            Thread thread = new Thread(new ParameterizedThreadStart(Talker.TalkToPlayerRandomCommand));
            thread.Start(new RandomDiceThreadParameters(name, playerState, number, RandomCommandSaver, PlayerAttributes, ChatGui));

        }

        public void Chat_OnFreeCompanyMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type != XivChatType.FreeCompany) return;
            var name = sender.TextValue;;
            var number = LotteryHelper.GetLotteryNumberRes(message);
            if (number == "") return;
            int intNum;
            bool success = int.TryParse(number, out intNum);
            if (!success) {
                moduleLog.Error("The Number can't not parse to int: {0}", number);
                return;
            }
            Thread thread = new Thread(new ParameterizedThreadStart(Talker.TalkInLotteryRes));
            thread.Start(new LotteryThreadParameters(intNum, name, number, LotterySaver));
        }

        public void Chat_OnSayMessageForInvite(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {   
            if (type != XivChatType.Say) return;
            moduleLog.Information("process to quene");
            var playerJob = PlayerAttributes.GetPlayerJob(sender.TextValue);
            if (playerJob == "") { return;}
            var senderPayload = sender.Payloads.Where(payload => payload is PlayerPayload).FirstOrDefault();
            if (senderPayload == null) { return; }
            if (senderPayload != default(Payload) && senderPayload is PlayerPayload playerPayload) { 
                inviter.QueneInvite(playerPayload, Talker);
            }
           
          
        }

        public void Chat_OnPartyMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type != XivChatType.Party) return;
            if (!message.TextValue.Contains("[跑团]")) return;
            var senderName = "";
            if (sender.TextValue.Length > 1) {
                senderName = sender.TextValue.Remove(0,1);
            }
            var playerState = RandomCommandSaver.GetPlayerState(senderName);
            moduleLog.Information("Playstate is: {0}, sender is {1}",playerState, senderName);
            
            if (playerState == "") {
                playerState = "0";
            };
            Thread thread = new Thread(new ParameterizedThreadStart(Talker.TalkToPlayer)); 
            thread.Start(new TalkToPlayerThreadParameters(senderName, playerState, message, RandomCommandSaver, ChatGui));
        }


        public RandomCommandSaver GetRandomCommandSaver() { return RandomCommandSaver; }
        public IDalamudPluginInterface GetInterface() { return PluginInterface; }

    }
}



