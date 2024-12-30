using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging.Internal;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule;

namespace RandomTalkPlugin.CommandTracker
{
    public class Talker
    {
        static Mutex Lotterylock = new Mutex();
        static Mutex RandomTalklock = new Mutex();
        static Mutex RandomDicelock = new Mutex();
        public string TalkSeq = "/wait 2";
        public string TalkLongSeq = "/wait 4";
        public string TargetToPlayer = "/ta <2>";
        public string ChoiceType = "选择";
        public unsafe Macro* macroLottery = RaptureMacroModule.Instance()->GetMacro(1, 0);
        public unsafe Macro* macroRandomDice = RaptureMacroModule.Instance()->GetMacro(1, 2);
        public unsafe Macro* macroRandomTalk = RaptureMacroModule.Instance()->GetMacro(1, 1);
        public string ServerName = "萌芽池";
        public string quenePlayerWord = "加入等待队列成功";
        public string mlock = "/mlock";
        public string endTalk = "/p 本段剧情已结束，可以回顾下剧情寻找下一个地点吧<wait.2>";
        public string tradeToPlayer = "/trade";
        public string kickPlayer = "/kick <2>";
        public ModuleLog moduleLog { get; set; }
        private RadomCommandHelper helper = new RadomCommandHelper();

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
                var mlockUtf8 = new Utf8String(mlock);
                var line1 = new Utf8String(TalkSeq);
                var line2 = new Utf8String(talkStr2);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 0, &mlockUtf8);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 1, &line1);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 2, &line2);
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
                        moduleLog.Information("end loop");
                        return;
                    }
                    if (textToSay.condition != null)
                    {
                        if (!threadParams.message.TextValue.Contains(textToSay.condition)) {
                            threadParams.ChatGui.Print("需要玩家打印：" + textToSay.condition + " 对应玩家: " + threadParams.playerName);
                            return;
                        }
                    }

                    (threadParams, ok) = SetChoiceRes(textToSay, threadParams, threadParams.playerState);
                    if (ok) { continue; }

                    var lineList = new List<string>();
                    foreach (var t in textToSay.text.Split("\n"))
                    {
                        if (t.Length > 60)
                        {
                            moduleLog.Error("对应文本一行超过60字符,请检查，场景为：{0}，玩家状态为：{1}", threadParams.RandomCommandSaver.CurrentSence, threadParams.playerState);
                            return;
                        }
                        var character = threadParams.RandomCommandSaver.GetCharacterName();
                        var vocie = "<se.16>";
                        if (textToSay.speaker != null)
                        {
                            character = textToSay.speaker;
                            vocie = "<se.14>";
                        }
                        lineList.Add("/p " + " [" + character + "]" + t + vocie + " <wait.4>");
                    }
                    if (lineList.Count > 12)
                    {
                        moduleLog.Error("对应文本总行数超过15行，文本行数不应该超过7行,请检查，场景为：{0}，玩家状态为：{1}", threadParams.RandomCommandSaver.CurrentSence, threadParams.playerState);
                        return;
                    }

                    if (textToSay.emotion == null ) 
                    {
                        textToSay.emotion = "说话";
                    }
                    var target = new Utf8String(TargetToPlayer);
                    var mlockUtf8 = new Utf8String(mlock);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, 0, &mlockUtf8);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, 1, &target);
                    var emotion = new Utf8String("/" + textToSay.emotion);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, 2, &emotion);
                    for (var i = 0; i < lineList.Count; i++)
                    {
                        var value = new Utf8String(lineList.ElementAt(i));
                        RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, i+3, &value);

                    }
                    RaptureShellModule.Instance()->ExecuteMacro(macroRandomTalk);
                    Thread.Sleep(textToSay.text.Split("\n").Count() * 4100);

                    if (textToSay.end) 
                    {
                        EndOperation(textToSay);
                    }
                    if (textToSay.successJump == null || textToSay.threasholdType != null || textToSay.thresholdValue != 0)
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

        private unsafe void EndOperation(TextToSay textToSay)
        {
            var endTalkUtf8 = new Utf8String(endTalk);
            var tradeUtf8 = new Utf8String(tradeToPlayer);
            var target = new Utf8String(TargetToPlayer);
            RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, 0, &target);
            if (textToSay.giftName != null) {
                RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, 1, &tradeUtf8);
            }
            RaptureMacroModule.Instance()->SetMacroLines(macroRandomTalk, 2, &endTalkUtf8);
            RaptureShellModule.Instance()->ExecuteMacro(macroRandomTalk);


        }

        public unsafe void TalkToPlayerRandomCommand(object parameters)
        {

            RandomDicelock.WaitOne();
            RandomDiceThreadParameters threadParams = (RandomDiceThreadParameters)parameters;
            try
            {
                var (textToSay, ok) = threadParams.RandomCommandSaver.GetTextToSayFromCharacterDialogue(threadParams.playerState);
                if (!ok) return;
                
                if (textToSay.thresholdValue == 0 || textToSay.threasholdType == ChoiceType) return;
                var rollAdd = "";
                var rollRes = "/p " + "roll点结果为:" + threadParams.number + " roll点成阈值为：" + textToSay.thresholdValue + "<wait.3>";
                var uf8Res = new Utf8String(rollRes);
                int intNum;
                if (Int32.TryParse(threadParams.number, out intNum))
                {
                    if (intNum > 20 )
                    {
                        threadParams.ChatGui.Print("点数roll的超过 20，请重新掷骰子:");
                        return;
                    }
                    var playerJob = threadParams.PlayerAttribute.GetPlayerJob(threadParams.playerName);
                    if (playerJob == null) {
                        threadParams.ChatGui.Print(threadParams.playerName + "玩家需要设置职业");
                    }
                    var playerAtt = threadParams.PlayerAttribute.GetJobAttribute(playerJob);
                    if (textToSay.threasholdType != null) 
                    {
                        if (textToSay.threasholdType == playerAtt.postiveAttribute.name)
                        {
                            var add = playerAtt.postiveAttribute.maxRandomNumber;
                            intNum = intNum + add;
                            rollAdd = "/p " + " 由于玩家职业为" + playerJob +  "," + textToSay.threasholdType + "属性加成为" + add + "，最后结果为:" + intNum;
                        }
                        else if (textToSay.threasholdType == playerAtt.negativeAttribute.name)
                        {
                            var sub = playerAtt.negativeAttribute.maxRandomNumber;
                            intNum = intNum - sub;
                            rollAdd = "/p " + " 由于玩家职业为" + playerJob + "," + textToSay.threasholdType + "属性加成为-" + sub + "，最后结果为:" + intNum;
                        } else
                        {
                            rollAdd = "/p " + " 由于玩家职业为" + playerJob + "," + textToSay.threasholdType + "属性加成为0，"  + "最后结果为:" + intNum;
                        }
                    }
                    moduleLog.Information("the rollAdd is " + rollAdd);

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
                    var addRes = new Utf8String(rollAdd);
                    var mlockUtf8 = new Utf8String(mlock);
                    RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 0, &mlockUtf8);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomDice, 1, &wait2);
                    RaptureMacroModule.Instance()->SetMacroLines(macroRandomDice, 2, &uf8Res);
                    if (rollAdd != "")
                    {
                        RaptureMacroModule.Instance()->SetMacroLines(macroRandomDice, 3, &addRes);
                    }

                    RaptureShellModule.Instance()->ExecuteMacro(macroRandomDice);
                    Thread.Sleep(5500);
                    Thread thread = new Thread(new ParameterizedThreadStart(TalkToPlayer));
                    thread.Start(new TalkToPlayerThreadParameters(threadParams.playerName, threadParams.playerState, new SeString { }, threadParams.RandomCommandSaver, threadParams.ChatGui));
                }
                
            }finally { RandomDicelock.ReleaseMutex();}


            

        }
        private (TalkToPlayerThreadParameters, bool) SetChoiceRes(TextToSay textToSay, TalkToPlayerThreadParameters threadParams, string playerState)
        {
            if (textToSay.threasholdType != ChoiceType)
            {
                return (threadParams, false);
            }
            
            if (threadParams.message.TextValue.Contains(textToSay.choice1Jump))
            {
                threadParams.playerState = textToSay.choice1Jump;
                threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName, textToSay.choice1Jump);
                return (threadParams, true);
            }
            if (threadParams.message.TextValue.Contains(textToSay.choice2Jump))
            {
                threadParams.playerState = textToSay.choice2Jump;
                threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName, textToSay.choice2Jump);
                return (threadParams, true);
            }
            threadParams.RandomCommandSaver.SetPlayerState(threadParams.playerName, playerState);
            return (threadParams, false);
            
        }

        public unsafe void TalkToQuenePlayerInSayChannel(List<string> quenePlayer,string currentPlayer)
        {
            RandomDicelock.WaitOne();
            RandomTalklock.WaitOne();
            try 
            { 
                string res = "/s " + currentPlayer + quenePlayerWord + ", 当前队列为";
                foreach (string p in quenePlayer) {
                    res += p + " ";
                }
                res += " " + "请勿重复添加队列，请保证自己不在其他队伍中";
                var resUtf8 = new Utf8String(res);
                var mlockUtf8 = new Utf8String(mlock);
                RaptureMacroModule.Instance()->SetMacroLines(macroLottery, 0, &mlockUtf8);
                RaptureMacroModule.Instance()->SetMacroLines(macroRandomDice, 1, &resUtf8);
                RaptureShellModule.Instance()->ExecuteMacro(macroRandomDice);
            }
            finally { RandomDicelock.ReleaseMutex(); RandomTalklock.ReleaseMutex(); }
            
        }

    }
}
