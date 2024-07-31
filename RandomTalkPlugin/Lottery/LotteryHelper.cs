using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Linq;
namespace RandomTalkPlugin.Lottery
{
    public class LotterydHelper
    {
        public string GetLotteryNumberRes(SeString message)
        {
            if (message.TextValue.Contains("我选") && message.TextValue.Contains("号"))
            {
                return message.TextValue.Split("我选")[1].Split("号")[0];
            }
            return "";
        }

        public (string, string) GetGiftRes(SeString message)
        {
            string[] words = message.ToString().Split(' ');
            if (message.TextValue.Contains("加入礼物") && words.Count() >= 3)
            {
                return (words.ElementAt(1), words.ElementAt(2));
            }
            return ("", "");
        }
    }

}
