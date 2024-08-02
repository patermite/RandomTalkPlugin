using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace RandomTalkPlugin.Windows;

public class RandomTalkWindow : Window, IDisposable
{
    private Configuration Configuration;
    int clusterSize;
    int daysShown = 90;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public RandomTalkWindow(RandomTalkPlugin plugin, PluginUI pluginUI) : base(plugin, pluginUI)
    {
        clusterSize = plugin.Configuration.ClusterSizeInHours;
    }

    public void Dispose() { }


    public override void Draw()
    {
        if (!Visible)
        {
            return;
        }
        ImGui.SetNextWindowSize(new Vector2(500, 500), ImGuiCond.FirstUseEver);
        if (ImGui.Begin("RandomTalk Controler", ref this.visible))
        {
            if (ImGui.InputInt("Amount of days shown", ref daysShown, 5, 30, ImGuiInputTextFlags.EnterReturnsTrue))
            {
            }
        }
        ImGui.End();
            
    }
}
