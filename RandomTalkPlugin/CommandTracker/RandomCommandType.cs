using RandomTalkPlugin.Lottery;
using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;

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
        public string threasholdTType;
        public int thresholdValue;
        public string successJump;
        public string failedJump;
        public string choice1Jump;
        public string choice2Jump;
        public string emotion;
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

        public RandomDiceThreadParameters(string playerName, string playerState, string number, RandomCommandSaver randomCommandSaver)
        {
            this.playerName = playerName;
            this.playerState = playerState;
            this.number = number;
            this.RandomCommandSaver = randomCommandSaver;
        }
    }

    class TalkToPlayerThreadParameters
    {
        public string playerName;
        public string playerState;
        public SeString message;
        public RandomCommandSaver RandomCommandSaver;

        public TalkToPlayerThreadParameters(string playerName, string playerState, SeString message, RandomCommandSaver randomCommandSaver)
        {
            this.playerName = playerName;
            this.playerState = playerState;
            this.message = message;
            this.RandomCommandSaver = randomCommandSaver;

        }
    }

}
