using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using RandomTalkPlugin;

namespace RandomTalkPlugin.Windows;

public class RandomTalkWindow : Window, IDisposable
{
    private Configuration Configuration;
    int clusterSize;

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
        Configuration = new Configuration { ClusterSizeInHours = clusterSize };
        var movable = Configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {

        }
    }
}
