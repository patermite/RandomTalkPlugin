using Dalamud.Plugin.Services;
using ImGuiNET;
using System.Linq;
using System.Numerics;

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


            ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.FirstUseEver);
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
                
                ImGui.EndChildFrame();


            }
            ImGui.End();
        }
    }
}
