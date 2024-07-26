using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using RandomTalkPlugin.Windows;
using RandomTalkPlugin.CommandTracker;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Logging;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using System;
using System.Runtime.InteropServices;
using Dalamud.Game.Text;
using static System.Net.Mime.MediaTypeNames;
using Dalamud.Plugin.Ipc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.String;
using System.Threading.Channels;


namespace RandomTalkPlugin
{
    public unsafe class RandomTalkPlugin : IDalamudPlugin
    {
        [PluginService] internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        

        private const string CommandName = "/randomtalk";
        public static RandomTalkPlugin Instance;
        public Configuration Configuration { get; init; }
        public IChatGui ChatGui { get; init; }

        public readonly WindowSystem WindowSystem = new("RandomTalkPlugin");
        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }
        private RadomCommandHelper CommandTracker { get; init; }
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr ParseMessageDelegate(IntPtr a, IntPtr b);
        private readonly Hook<ParseMessageDelegate> parseMessageHook;
        private delegate void MacroCallDelegate(RaptureShellModule* raptureShellModule, RaptureMacroModule.Macro* macro);
        private readonly XivChatType ownRollType = (XivChatType)2122;
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

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this, goatImagePath);
            CommandTracker = new RadomCommandHelper();

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });
            this.ChatGui.ChatMessage += Chat_OnRandomDiceMessage;
            // PluginLog.Information(CommandTracker.GetRandomCommandRes());

            PluginInterface.UiBuilder.Draw += DrawUI;

            // This adds a button to the plugin installer entry of this plugin which allows
            // to toggle the display status of the configuration ui
            PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

            // Adds another button that is doing the same but for the main ui of the plugin
            PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();

            ConfigWindow.Dispose();
            MainWindow.Dispose();

            CommandManager.RemoveHandler(CommandName);
            this.ChatGui.ChatMessage -= Chat_OnRandomDiceMessage;
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just toggle the display status of our main ui
            ToggleMainUI();
        }

        private void DrawUI() => WindowSystem.Draw();

        public void ToggleConfigUI() => ConfigWindow.Toggle();
        public void ToggleMainUI() => MainWindow.Toggle();

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
    }
}



