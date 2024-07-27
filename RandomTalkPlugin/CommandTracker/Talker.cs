using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

using RandomTalkPlugin.Lottery;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using System.Numerics;

namespace RandomTalkPlugin.CommandTracker
{
    public class Talker
    {
        public DateTime TimeStamp { get; set; }
        public long Number { get; set; }
        public long Total { get; set; }
        public IChatGui ChatGui { get; init; }
       


        public unsafe void TalkInLotteryRes(int intNum, string name,string number, LotterydSaver LotterySaver)
        {
            var (giftSender, giftName) = LotterySaver.GetGift(intNum, name);
            string talkStr1, talkStr2;
            talkStr1 = "/wait 2";
            var giftDcit = LotterySaver.GetGiftDict();
            var numberArray = giftDcit.Keys.ToArray();
            if (giftSender == "")
            {
                talkStr2 = "/fc " + name + "选择的是" + number + "号，但是礼物库中没有此号码，请重新选择！";
            }
            else
            {
                talkStr2 = "/fc " + name + "选择的是" + number + "号，他抽中来自" + giftSender + "的礼物：" + giftName + "！剩余号码为" +
                    string.Join(" ", Array.ConvertAll(numberArray, x => x.ToString()));
            }
            var line1 = new Utf8String(talkStr1);
            var line2 = new Utf8String(talkStr2);
            var macro = RaptureMacroModule.Instance()->GetMacro(1, 0);
            RaptureMacroModule.Instance()->SetMacroLines(macro, 0, &line1);
            RaptureMacroModule.Instance()->SetMacroLines(macro, 1, &line2);
            RaptureShellModule.Instance()->ExecuteMacro(macro);

        }

    }
}
