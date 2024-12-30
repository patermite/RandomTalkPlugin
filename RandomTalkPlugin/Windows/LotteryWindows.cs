
using ImGuiNET;
using System.Numerics;


namespace RandomTalkPlugin.Windows
{
    public class LotteryWindows : Windows
    {
        int clusterSize;
        string inputPlayerName = "玩家名字";
        string inputGiftName = "礼物名字";       
        public LotteryWindows(RandomTalkPlugin plugin, PluginUI pluginUI) : base(plugin, pluginUI)
        {
            clusterSize = plugin.Configuration.ClusterSizeInHours;
        }


        public override void Draw()
        {
            if (!Visible)
            {
                return;
            }
            var configValue = this.plugin.Configuration.SomePropertyToBeSavedAndWithADefault;


           

            ImGui.SetNextWindowSize(new Vector2(232, 232), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Lottery Controller", ref this.visible))
            {
                if (ImGui.Checkbox("开启抽奖插件", ref this.plugin.StartLotterySwitch))
                {
                    if (this.plugin.StartLotterySwitch)
                    {
                        this.plugin.ChatGui.ChatMessage += this.plugin.Chat_OnFreeCompanyMessage;
                        this.plugin.StartLotterySwitch = true;
                        this.plugin.ChatGui.Print("Start Lottery set on sucess");

                    }
                    else
                    {
                        this.plugin.ChatGui.ChatMessage -= this.plugin.Chat_OnFreeCompanyMessage;
                        this.plugin.StartLotterySwitch = false;
                        this.plugin.ChatGui.Print("Start Lottery set off sucess");
                    }

                }
                if (ImGui.Button("Print Gifts"))
                {
                    this.plugin.ChatGui.Print( "礼物列表");
                    var giftDict = this.plugin.LotterySaver.GetGiftDict();
                    foreach (var (number, (name,giftName)) in giftDict)
                    {
                        this.plugin.ChatGui.Print("号码:"+ number +" "+ name + "：" + giftName);
                    }
                    
                }

                if (ImGui.Button("Print GiftsDesitination"))
                {
                    this.plugin.ChatGui.Print("礼物去向");
                    var desDict = this.plugin.LotterySaver.GetDestinationGiftDict();
                    foreach (var ((name, giftName), recevier) in desDict)
                    {
                        this.plugin.ChatGui.Print(name + ", " + giftName+ "："+ recevier);
                    }
                }

                if (ImGui.Button("Eport to csv"))
                {
                    plugin.LotterySaver.ExportToCsv(this.plugin.ChatGui);
                }

                ImGui.InputText("##PlayerName", ref inputPlayerName, 100);
                ImGui.InputText("##GiftName", ref inputGiftName, 100);

                ImGui.Text("将加入" + inputPlayerName+"的礼物：" + inputGiftName);
                if (ImGui.Button("Add Gift"))
                {
                    this.plugin.LotterySaver.SetGift(inputPlayerName, inputGiftName);
                }

                if (ImGui.Button("Load Gift"))
                {
                    plugin.LotterySaver.LoadPlayerGift(this.plugin.GetInterface());
                }


                ImGui.EndChildFrame();


            }
            ImGui.End();
        }
    }
}
