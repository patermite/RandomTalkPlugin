using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using Lumina.Excel.GeneratedSheets;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using static FFXIVClientStructs.FFXIV.Client.Game.UI.MapMarkerContainer;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule;

namespace RandomTalkPlugin.CommandTracker
{
    public class Talker
    {
        static Mutex Lotterylock = new Mutex();
        static Mutex RandomTalklock = new Mutex();
        static Mutex RandomDicelock = new Mutex();
        public string TalkSeq = "/wait 2";
        public string TargetToPlayer = "/tpc";
        public unsafe Macro* macroLottery = RaptureMacroModule.Instance()->GetMacro(1, 0);
        public unsafe Macro* macroRandomDice = RaptureMacroModule.Instance()->GetMacro(2, 2);
        public unsafe Macro* macroRandomTalk = RaptureMacroModule.Instance()->GetMacro(1, 1);
        public string ServerName = "萌芽池";

        public unsafe void TalkInLotteryRes(object parameters)
        {
            Lotterylock.WaitOne();
            try
            {
                LotteryThreadParameters threadParams = (LotteryThreadParameters)parameters;
                var (giftSender, giftName) = threadParams.LotterySaver.GetGift(threadParams.intNum, threadParams.name);
                string talkStr2;

                var giftDcit = threadParams.LotterySaver.GetGiftDict();
                var numberArray = giftDcit.Keys.ToArray();
                if (giftSender == "")
                {
                    talkStr2 = "/fc " + threadParams.name + "选择的是" + threadParams.number + "号，但是礼物库中没有此号码，请重新选择！";
                }
                else
                {
                    talkStr2 = "/fc " + threadParams.name + "选择的是" + threadParams.number + "号，他抽中来自" + giftSender + "的礼物：" + giftName + "！剩余号码为" +
                        string.Join(" ", Array.ConvertAll(numberArray, x => x.ToString()));
                }

                var line1 = new Utf8String(TalkSeq);
                var line2 = new Utf8String(talkStr2);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 0, &line1);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 1, &line2);
                RaptureShellModule.Instance()->ExecuteMacro(macroLottery);
                Thread.Sleep(3000);
            }
            finally {
                Lotterylock.ReleaseMutex();
            }
        }

        public unsafe void TalkToPlayer(object parameters)
        {
            RandomTalklock.WaitOne();
            TalkToPlayerThreadParameters threadParams = (TalkToPlayerThreadParameters)parameters;
            try
            {
                for (int loop = 0;loop < 9; loop++) 
                {
                    var (textToSay, ok) = threadParams.RandomCommandSaver.GetTextToSayFromCharacterDialogue(threadParams.playerState);
                    if (!ok)
                    {
                        PluginLog.Information("end loop");
                        return;
                    }
                    if (textToSay.condition != null)
                    {
                        if (!threadParams.message.TextValue.Contains(textToSay.condition)) {
                            PluginLog.Information("need condition" + textToSay.condition + " for player: " + threadParams.playerName);
                            return;
                        }
                    }

                    var lineList = new List<string>();
                    PluginLog.Information("the text to say text is :" + textToSay.text);
                    foreach (var t in textToSay.text.Split("\n"))
                    {
                        if (t.Length > 50)
                        {
                            PluginLog.Error("对应文本一行超过60字符,请检查，场景为：{0}，玩家状态为：{1}", threadParams.RandomCommandSaver.CurrentSence, threadParams.playerState);
                            return;
                        }
                        lineList.Add("/t " + threadParams.playerName + "@" + ServerName + " [" + threadParams.RandomCommandSaver.GetCharacterName() + "]" + t + " <wait.3>");
                    }
                    if (lineList.Count > 13)
                    {
                        PluginLog.Error("对应文本总行数超过15行，文本行数不应该超过7行,请检查，场景为：{0}，玩家状态为：{1}", threadParams.RandomCommandSaver.CurrentSence, threadParams.playerState);
                        return;
                    }

                    if (textToSay.emotion == null )
                    {
                        textToSay.emotion = "说话";
                     }
                    var firstLine = new Utf8String(TargetToPlayer);
                    RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 0, &firstLine);
                    var secondLine = new Utf8String("/" + textToSay.emotion);
                    RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 1, &secondLine);
                    for (var i = 0; i < lineList.Count; i++)
                    {
                        var value = new Utf8String(lineList.ElementAt(i));
                        RaptureMacroModule.Instance()->SetMacroLines(macroLottery, i+2, &value);

                    }
                    RaptureShellModule.Instance()->ExecuteMacro(macroLottery);
                    Thread.Sleep(textToSay.text.Split("\n").Count() * 3100);
                    if (textToSay.threasholdTType == "Choice")
                    {
                        if (threadParams.message.TextValue.Contains(textToSay.choice1Jump))
                        {
                            threadParams.playerState = textToSay.choice1Jump;
                            threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName, textToSay.choice1Jump);
                            continue;
                        }
                        if (threadParams.message.TextValue.Contains(textToSay.choice2Jump))
                        {
                            threadParams.playerState = textToSay.choice2Jump;
                            threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName, textToSay.choice2Jump);
                            continue;
                        }
                        return;
                    }

                    if (textToSay.successJump == null || textToSay.threasholdTType != null || textToSay.thresholdValue != 0)
                    {
                        break;
                    }
                    threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName,textToSay.successJump);
                    threadParams.playerState = textToSay.successJump;
                }                
            }finally
            {
                RandomTalklock.ReleaseMutex();
            }

        }

        public unsafe void TalkToPlayerRandomCommand(object parameters)
        {

            RandomDicelock.WaitOne();
            RandomDiceThreadParameters threadParams = (RandomDiceThreadParameters)parameters;
            try
            {
                var (textToSay, ok) = threadParams.RandomCommandSaver.GetTextToSayFromCharacterDialogue(threadParams.playerState);
                if (!ok) return;
                
                if (textToSay.thresholdValue == 0) return;
                var rollRes = "/t " + threadParams.playerName + "@" + ServerName + " " + "roll点结果为:" + threadParams.number + " roll点成阈值为：" + textToSay.thresholdValue + "<wait.3>";
                var uf8Res = new Utf8String(rollRes);
                int intNum;
                if (Int32.TryParse(threadParams.number, out intNum))
                {
                    if (intNum > 20)
                    {
                        PluginLog.Error("The roll number over 20:");
                        return;
                    }
                    if (intNum >= textToSay.thresholdValue)
                    {
                        threadParams.playerState = textToSay.successJump;
                    }
                    else
                    {
                        threadParams.playerState = textToSay.failedJump;
                    }
                    threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName, threadParams.playerState);
                    var wait2 = new Utf8String(TalkSeq);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomDice, 0, &wait2);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomDice, 1, &uf8Res);
                    RaptureShellModule.Instance()->ExecuteMacro(macroRandomDice);
                    Thread.Sleep(1000);
                    Thread thread = new Thread(new ParameterizedThreadStart(TalkToPlayer));
                    thread.Start(new TalkToPlayerThreadParameters(threadParams.playerName, threadParams.playerState, new SeString { }, threadParams.RandomCommandSaver));
                }
                
            }finally { RandomDicelock.ReleaseMutex();}
            
            

        }

    }
}
