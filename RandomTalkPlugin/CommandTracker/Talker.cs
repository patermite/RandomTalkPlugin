using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using RandomTalkPlugin.Lottery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule;

namespace RandomTalkPlugin.CommandTracker
{
    public class Talker
    {

        static object Lotterylock = new object();
        static object RandomTalklock = new object();
        public string CurrentSence = "场景一";
        public string TalkSeq = "/wait 2";
        public unsafe Macro* macroLottery = RaptureMacroModule.Instance()->GetMacro(1, 0);
        public unsafe Macro* macroRandomTalk = RaptureMacroModule.Instance()->GetMacro(1, 1);
        public RandomCommandSaver RandomCommandSaver = new RandomCommandSaver();
        public string ServerName = "萌芽池";

        public unsafe void TalkInLotteryRes(int intNum, string name, string number, LotterydSaver LotterySaver)
        {
            lock (Lotterylock)
            {
                var (giftSender, giftName) = LotterySaver.GetGift(intNum, name);
                string talkStr2;

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

                var line1 = new Utf8String(TalkSeq);
                var line2 = new Utf8String(talkStr2);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 0, &line1);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 1, &line2);
                RaptureShellModule.Instance()->ExecuteMacro(macroLottery);
                Thread.Sleep(3000);
            }

        }

        public unsafe void TalkToPlayer(string playerName, string playerState)
        {
            lock (RandomTalklock)
            {
                var (textToSay, ok) = RandomCommandSaver.GetTextToSayFromCharacterDialogue(CurrentSence, playerState);
                if (!ok)
                {
                    PluginLog.Error("没有找到对应场景的文本，场景为：{0}，玩家状态为：{1}", CurrentSence, playerState);
                    return;
                }
                var lineList = new List<string>();
                foreach (var t in textToSay.text.Split("\n"))
                {
                    lineList.Add(TalkSeq);
                    if (t.Length > 60)
                    {
                        PluginLog.Error("对应文本一行超过60字符,请检查，场景为：{0}，玩家状态为：{1}", CurrentSence, playerState);
                        return;
                    }
                    lineList.Add("/t " + playerName + "@" + ServerName + " " + t);
                }
                if (lineList.Count > 15)
                {
                    PluginLog.Error("对应文本总行数超过15行，文本行数不应该超过7行,请检查，场景为：{0}，玩家状态为：{1}", CurrentSence, playerState);
                    return;
                }
                for (var i = 0; i < lineList.Count; i++)
                {
                    var value = new Utf8String(lineList.ElementAt(i));
                    RaptureMacroModule.Instance()->SetMacroLines(macroLottery, i, &value);

                }
                RaptureShellModule.Instance()->ExecuteMacro(macroLottery);
            };

        }

    }
}
