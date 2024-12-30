using RandomTalkPlugin.Lottery;
using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RandomTalkPlugin.DataManager
{
    public struct PlayerStoryData
    {
        public string playerName;
        public List<PlayerStoryResultPoint> storyResultPointList;
    }

    public struct PlayerStoryResultPoint 
    {
        public string storyName;
        public string storyResult;

    }

    public struct PlayerBattleData
    {
        public string playerName;
        public Dictionary<string, PlayerStoryResultPoint> battleResultPointList;
    }

}
