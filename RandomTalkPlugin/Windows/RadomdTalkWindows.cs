using System;
using System.Numerics;
using ImGuiNET;

namespace RandomTalkPlugin.Windows;

public class RandomTalkWindow : Windows, IDisposable
{
    private Configuration Configuration;
    int clusterSize;
    string inputPlayerName = "玩家名字";
    string inputState = "玩家存档位置";
    string inputJobPlayerName = "设置职业的玩家名字";
    string inputJob = "职业名";
    string inputCharacterFileName = "角色文件名不带后缀";

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
        int selectedItem = 0;
        ImGui.SetNextWindowSize(new Vector2(500, 500), ImGuiCond.FirstUseEver);
        if (ImGui.Begin("RandomTalk Controler", ref this.visible))
        {
            ImGui.BeginDragDropSource(ImGuiDragDropFlags.None);
            var rp = this.plugin.GetRandomCommandSaver();
            var senceList = rp.GetAllSences();
            if (ImGui.Checkbox("开启角色插件", ref this.plugin.StartCharacterTalkSwitch))
            {
                // 当复选框状态发生变化时的逻辑处理
                if (this.plugin.StartCharacterTalkSwitch)
                {
                    if (!this.plugin.GetRandomCommandSaver().CheckCharacterDialogue()) return;
                    this.plugin.ChatGui.ChatMessage += this.plugin.Chat_OnPartyMessage;
                    this.plugin.ChatGui.ChatMessage += this.plugin.Chat_OnRandomDiceMessage;
                    this.plugin.ChatGui.ChatMessage += this.plugin.Chat_OnSayMessageForInvite;
                    this.plugin.StartCharacterTalkSwitch = true;
                    this.plugin.ChatGui.Print("Start Character Talk set on sucess");
                }
                else
                {
                    this.plugin.ChatGui.ChatMessage -= this.plugin.Chat_OnPartyMessage;
                    this.plugin.ChatGui.ChatMessage -= this.plugin.Chat_OnRandomDiceMessage;
                    this.plugin.ChatGui.ChatMessage -= this.plugin.Chat_OnSayMessageForInvite;
                    this.plugin.StartCharacterTalkSwitch = false;
                    this.plugin.ChatGui.Print("Start Character Talk set off sucess");
                }

            }
            ImGui.InputText("##CharacterFileName", ref inputCharacterFileName, 20);
            ImGui.SameLine();
            if (ImGui.Button("导入角色"))
            {
                rp.SetCharacterName(inputCharacterFileName);
                this.plugin.ChatGui.Print("修改角色为" + inputCharacterFileName);
                rp.LoadCharacterDialogue(this.plugin.GetInterface());
            }

            if (ImGui.BeginCombo("##场景选择", rp.CurrentSence, 0))
            {
                for (int i = 0; i < senceList.Count; i++)
                {
                    bool isSelected = selectedItem == i;
                    
                    if (ImGui.Selectable(senceList[i], isSelected))
                    {
                        selectedItem = i;
                        rp.SetCurrentSence(senceList[i]);
                    }
                    if (isSelected)
                    {
                        
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }

            

            ImGui.InputText("##PlayerName", ref inputPlayerName, 20);
            ImGui.SameLine();
            ImGui.InputText("##State", ref inputState, 20);
            if (ImGui.Button("修改玩家存档"))
            {
                this.plugin.GetRandomCommandSaver().SetPlayerState(inputPlayerName, inputState);
                this.plugin.ChatGui.Print("修改玩家" + inputPlayerName + "的存档为" + inputState);
            }

            ImGui.InputText("##JobPlayerName", ref inputJobPlayerName, 20);
            ImGui.SameLine();
            ImGui.InputText("##Job", ref inputJob, 20);
            if (ImGui.Button("设置玩家职业"))
            {
                this.plugin.PlayerAttributes.SetPlayerJob(inputJobPlayerName, inputJob);
                this.plugin.ChatGui.Print("修改玩家" + inputJobPlayerName + "的职业为" + inputJob);
            }

            //if (ImGui.InputInt("Amount of days shown", ref daysShown, 5, 30, ImGuiInputTextFlags.EnterReturnsTrue))
            if (ImGui.Button("打印当前场景"))
            {
                var currentSence = this.plugin.GetRandomCommandSaver().CurrentSence; ;

                this.plugin.ChatGui.Print("当前场景为： " + currentSence);
            }

            if (ImGui.Button("打印当前玩家存档"))
            {
                var playerStates = this.plugin.GetRandomCommandSaver().GetAllPlayerStates();
                foreach (var (name, state) in playerStates)
                {
                    this.plugin.ChatGui.Print(name + "的状态为：" + state);
                }
            }

            if (ImGui.Button("打印当前玩家职业"))
            {
                var playerJobs = this.plugin.PlayerAttributes.GetAllPlayerJob();
                foreach (var (name, job) in playerJobs)
                {
                    this.plugin.ChatGui.Print(name + "的职业为：" + job);
                }
            }

        }
        ImGui.End();
            
    }
}
