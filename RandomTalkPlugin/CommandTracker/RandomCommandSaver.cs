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
        public string CurrentSence = "场景一";
        public string CharacterName = "SampleRandomTalk";
        public void LoadCharacterDialogue(DalamudPluginInterface pluginInterface)
        {
            if (CharacterName == "") { return; }
            PluginLog.Information("the path is:" + pluginInterface.ConfigDirectory.FullName + $"\\" + CharacterName+".json");
            if (!File.Exists(pluginInterface.ConfigDirectory.FullName + $"\\" + CharacterName + ".json")) return;
            string content = File.ReadAllText(Path.Join(pluginInterface.ConfigDirectory.FullName, CharacterName+".json"));
            CharacterDialogue = JsonConvert.DeserializeObject<CharacterDialogue>(content);
            if (CharacterDialogue.dialogue != null && CharacterDialogue.dialogue.Count > 0)
            {
                CurrentSence = CharacterDialogue.dialogue[0].scenename;
            }
            return;
        }
        public void SetCharacterName (string name)
        {
            if (name == null) { return; }
            CharacterName = name;
        }

        public (TextToSay, bool) GetTextToSayFromCharacterDialogue(string keyword)

        {
            try
            {
                foreach (var s in CharacterDialogue.dialogue)
                {
                    if (s.scenename == CurrentSence)
                    {                      
                        if (s.texttosay.TryGetValue(keyword, out TextToSay value))
                        {
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
            if (state == null)
            {
                return;
            }
            playerStates[playerName] = state;
        }

        public string GetPlayerState(string playerName)
        {
            if (playerStates.TryGetValue(playerName, out var value)) { return value; }
            return "";
        }

        public string GetCharacterName()
        {
            return CharacterDialogue.character;
        }

        public bool CheckCharacterDialogue()
        {
            if (CharacterDialogue.character == null) { PluginLog.Error("check character failed"); return false; }

            if (CharacterDialogue.dialogue == null) { PluginLog.Error("check dialogue failed"); return false; }
            return true;

        }

        public Dictionary<string, string> GetPlayStateDict() { return playerStates; }
        
        public List<string> GetAllSences()
        {
            var res = new List<string>();
            if (CharacterDialogue.dialogue == null) {
                return res;
            }
            foreach (var sence in CharacterDialogue.dialogue)
            {
                res.Add(sence.scenename);
            }
            return res;
        }
        public Dictionary<string, string> GetAllPlayerStates()
        {
            return playerStates;
        }


        public void SetCurrentSence(string sence)
        {
            var senceList = GetAllSences();
            foreach (var s in senceList)
            {
                if (sence == s)
                {
                    CurrentSence = sence;
                    playerStates = new Dictionary<string, string> { };
                    return;
                }
            }
            playerStates = new Dictionary<string, string> { };
        }

    }
}
