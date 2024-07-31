using System;
using System.Collections.Generic;
using System.IO;
using Dalamud.Plugin;
using Newtonsoft.Json;


namespace RandomTalkPlugin.CommandTracker
{
    public class RandomCommandSaver
    {
        public CharacterDialogue CharacterDialogue = new CharacterDialogue();
        private Dictionary<string, string> playerStates = new Dictionary<string, string> { };
        public void LoadCharacterDialogue(DalamudPluginInterface pluginInterface)
        {
            string content = File.ReadAllText(Path.Join(pluginInterface.ConfigDirectory.FullName, $"character.json"));
            CharacterDialogue = JsonConvert.DeserializeObject<CharacterDialogue>(content);
            return;
        }

        public (TextToSay, bool) GetTextToSayFromCharacterDialogue(string senceName, string keyword)
            
        {
            foreach (var s in CharacterDialogue.scenes)
            {
                if (s.sceneName == senceName) {
                    if (s.textToSay.TryGetValue(keyword, out TextToSay value)) {
                        return (value, true);
                    }
                    break;
                } 
            }
            return (new TextToSay { }, false);
        }

        public void SetPlayerState(string playerName, string state)
        {
            playerStates[playerName] = state;
        }

        public string GetPlayerState(string playerName)
        {
            return playerStates[playerName];
        }

    }
        
}
