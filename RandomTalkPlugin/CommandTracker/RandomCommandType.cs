using RandomTalkPlugin.Lottery;
using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace RandomTalkPlugin.CommandTracker
{
    public struct CharacterDialogue
    {
        public string character;
        public List<Scene> dialogue;
    }

    public struct Scene
    {
        public string scenename;
        public Dictionary<string, TextToSay> texttosay;
    }

    public struct TextToSay
    {
        public string condition;
        public string text;
        public string threasholdType;
        public int thresholdValue;
        public string successJump;
        public string failedJump;
        public string choice1Jump;
        public string choice2Jump;
        public string emotion;
        public string speaker;
    }

    class LotteryThreadParameters
    {
        public int intNum;
        public string name;
        public string number;
        public LotterydSaver LotterySaver;

        public LotteryThreadParameters(int intNum, string name, string number, LotterydSaver LotterySaver)
        {
            this.intNum = intNum;
            this.name = name;
            this.number = number;
            this.LotterySaver = LotterySaver;
        }
    }

    class RandomDiceThreadParameters
    {
        public string playerName;
        public string playerState;
        public string number;
        public RandomCommandSaver RandomCommandSaver;
        public PlayerAttribute PlayerAttribute;
        public IChatGui ChatGui;

        public RandomDiceThreadParameters(string playerName, string playerState, string number, RandomCommandSaver randomCommandSaver, PlayerAttribute playerAttribute, IChatGui chatGui)
        {
            this.playerName = playerName;
            this.playerState = playerState;
            this.number = number;
            this.RandomCommandSaver = randomCommandSaver;
            this.PlayerAttribute = playerAttribute;
            this.ChatGui = chatGui;
        }
    }

    class TalkToPlayerThreadParameters
    {
        public string playerName;
        public string playerState;
        public SeString message;
        public RandomCommandSaver RandomCommandSaver;
        public IChatGui ChatGui;

        public TalkToPlayerThreadParameters(string playerName, string playerState, SeString message, RandomCommandSaver randomCommandSaver, IChatGui chatGui)
        {
            this.playerName = playerName;
            this.playerState = playerState;
            this.message = message;
            this.RandomCommandSaver = randomCommandSaver;
            this.ChatGui = chatGui; 
        }
    }

    public struct JobAttributes
    {
        public AttributesInfo postiveAttribute;
        public AttributesInfo negativeAttribute;
    }

    public struct AttributesInfo
    {
        public string name;
        public int maxRandomNumber;
    }

    public struct EquipInfo
    {
        public string attributeName;
        public int attributeNumber;
    }

}
