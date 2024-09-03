using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Ipc;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Dalamud.Logging;
using System.IO;
using Newtonsoft.Json;

namespace RandomTalkPlugin.CommandTracker
{


    public unsafe class PlayerAttribute
    {
        public IClientState ClientState { get; init; } = null!;
        public Dictionary<string, string> playerJob = new Dictionary<string, string> { };
        private Dictionary<string, List<string>> playerEquip = new Dictionary<string, List<string>> { };
        public static ITargetManager TargetManager {  get; set; } = null!;
        public static TargetSystem targetSystem { get; set; }
        public string jobFileName = "PlayerJob";
        public static Dictionary<string, JobAttributes> JobAttribute = new Dictionary<string, JobAttributes>
        {
            { "战士", new JobAttributes
                {
                    postiveAttribute = new AttributesInfo { name = "力量", maxRandomNumber = 4 },
                    negativeAttribute = new AttributesInfo { name = "学识", maxRandomNumber = 4 }
                }
            },
            { "诗人",new JobAttributes 
                {
                    postiveAttribute = new AttributesInfo { name = "说服", maxRandomNumber = 4 },
                    negativeAttribute = new AttributesInfo { name = "力量", maxRandomNumber = 4 }
                }
            },
            {"学者" , new JobAttributes
                {
                    postiveAttribute = new AttributesInfo { name = "学识", maxRandomNumber = 4 },
                    negativeAttribute = new AttributesInfo { name = "说服", maxRandomNumber = 4 }
                } 
            }
        };

        public void LoadPlayerJob(DalamudPluginInterface pluginInterface)
        {
            PluginLog.Information("the path of player job is:" + pluginInterface.ConfigDirectory.FullName + $"\\" + jobFileName + ".json");
            if (!File.Exists(pluginInterface.ConfigDirectory.FullName + $"\\" + jobFileName + ".json")) return;
            string content = File.ReadAllText(Path.Join(pluginInterface.ConfigDirectory.FullName, jobFileName + ".json"));
            if (content == null) return;
            playerJob = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            return;
        }

        public static Dictionary<string, EquipInfo> EquipDict = new Dictionary<string, EquipInfo>
        {
            { "白色花环", new EquipInfo
                {
                    attributeName = "力量", attributeNumber = 4
                }
            }
        };

        public void SetPlayerJob(string playerName, string attribute)
        {
            if (attribute == null)
            {
                return;
            }
            playerJob[playerName] = attribute;
        }


        public string GetPlayerJob(string playerName)
        {
            if (playerJob.TryGetValue(playerName, out var value)) { return value; }
            return "";
        }

        public void SetPlayerEquip(string playerName, string equip)
        {
            if (equip == null)
            {
                return;
            }
            var tempList = new List<string>();
            if (playerEquip.TryGetValue(playerName, out var value))
            {
                foreach ( var item in value)
                {
                    if (item == equip) {
                        return;
                    }
                }
                value.Add(equip);
                tempList = value;
            } else
            {
                tempList.Add(equip);
            }
            playerEquip[playerName] = tempList;
        }

        public List<string> GetPlayerEquip(string playerName)
        {
            if (playerEquip.TryGetValue(playerName, out var value)) { return value; }
            return new List<string>();
        }

        public Dictionary<string, string> GetAllPlayerJob()
        {
            return playerJob;
        }

        public JobAttributes GetJobAttribute(string playerName)
        {
            if (JobAttribute.TryGetValue(playerName, out var value)) { return value; }
            return new JobAttributes();
        }


        public static bool UseRepair() => ActionManager.Instance()->UseAction(ActionType.GeneralAction, 6);

        public void ChoosePlayer() 
        {
            if (this.ClientState.LocalPlayer!.HomeWorld.GameData == null) {  return; }
            var world = this.ClientState.LocalPlayer!.HomeWorld.GameData.Name.RawString;
            PluginLog.Information("the world name is :" + world);
            var targetObject = this.ClientState.LocalPlayer!.TargetObject;
            if (targetObject == null) { return; }
            PluginLog.Information("the target object:"+ targetObject.Name.TextValue);
        }
    }

}
