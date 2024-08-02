
using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Interface.Windowing;


namespace RandomTalkPlugin.Windows
{
    public class LotteryWindows : Window
    {
        int clusterSize;

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


            ImGui.SetNextWindowSize(new Vector2(232, 232), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Lottery Controller", ref this.visible))
            {
                if (ImGui.Button("Print Gifts"))
                {
                    var giftDict = this.plugin.LotterySaver.GetGiftDict();
                    foreach (var (_, (name,giftName)) in giftDict)
                    {
                        this.plugin.ChatGui.Print(name + "：" + giftName);
                    }
                    
                }

                if (ImGui.Button("Print GiftsDesitination"))
                {
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

                ImGui.EndChildFrame();


            }
            ImGui.End();
        }
    }
}
