using System;
using System.Collections.Generic;
using System.IO;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Newtonsoft.Json;


namespace RandomTalkPlugin.CommandTracker
{
    public class RandomCommandSaver
    {
        public CharacterDialogue CharacterDialogue = new CharacterDialogue();
        private Dictionary<string, string> playerStates = new Dictionary<string, string> { };
        public void LoadCharacterDialogue(DalamudPluginInterface pluginInterface)
        {
            PluginLog.Information("the path is:" + pluginInterface.ConfigDirectory.FullName + $"\\SampleRandomTalk.json");
            if (!File.Exists(pluginInterface.ConfigDirectory.FullName + $"\\SampleRandomTalk.json")) return;
            string content = File.ReadAllText(Path.Join(pluginInterface.ConfigDirectory.FullName, $"SampleRandomTalk.json"));
            CharacterDialogue = JsonConvert.DeserializeObject<CharacterDialogue>(content);
            return;
        }

        public (TextToSay, bool) GetTextToSayFromCharacterDialogue(string senceName, string keyword)

        {
            try
            {
                foreach (var s in CharacterDialogue.dialogue)
                {
                    PluginLog.Information($"keyword: {keyword}, textToSay is {s.texttosay}");
                    if (s.scenename == senceName)
                    {                      
                        if (s.texttosay.TryGetValue(keyword, out TextToSay value))
                        {
                            PluginLog.Information("find it");
                            return (value, true);
                        }
                        break;
                    }
                }
            }
            catch (NullReferenceException ex) { PluginLog.Error("Error: " + ex.Message); };

            return (new TextToSay { }, false);
        }

        public void SetPlayerState(string playerName, string state)
        {
            playerStates[playerName] = state;
        }

        public string GetPlayerState(string playerName)
        {
            if (playerStates.TryGetValue(playerName, out var value)) { return value; }
            return "";
        }

        public bool CheckCharacterDialogue()
        {
            if (CharacterDialogue.character == null) { PluginLog.Error("check character failed"); return false; }

            if (CharacterDialogue.dialogue == null) { PluginLog.Error("check dialogue failed"); return false; }
            return true;

        }

    }
}
