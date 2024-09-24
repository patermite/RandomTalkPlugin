

# RandomTalkPlugin

Simple talk plugin for role player in FFXIV, it can talk like NPC use specifc json file.

For more detailed questions, just leave a issue in this repository, I will response the issue sometime.

## Main Points

Simple function can make your player talk like NPC , using by preset logic. Customize your speaking logic and interaction logic

## How To Use
1. install the plugin, use a location that your Dalmud can scan it
2. After scan you can use "/randomtal"open the consle of this plugin
3. Load the character file before your start use the plugin, put the character json file into the plugin config, usually the path is \XIVLauncherCN\Roaming\pluginConfigs\RandomTalkPlugin, if not exist just create it, the character file you can finded it in the \Data folder.
4. Load the json using the plugin consle, by input the file name without file suffix, for example, input the "SampleRandomTalk" and click the "导入角色" button.
5. select the correct sence and enable the plugin

### Getting Started
Thie chapter just show the logic of json file, you can see the table below, you can customize your own NPC by write the json file.

| Fields      | Description |
| :---        |  ----:      |
| character   | Character name can display in every beginning of the sentence.(String)|
| scenename    | The scene name of a specific character.(String)|
| texttosay    | A dictionary containing text that needs to be said for a specific scene and a specific character(String)|
| texttosay.keys    | A unique key for the step of the text(String)|
| text    | The text need to say, "\n" can sperate the string, and NPC will speak it by line to line(String).|
| successJump    | A keyword of the next text need to say in this scene, it just point to the key in this texttosay(String)|
| emotion    | Speak while using the corresponding emotion action.(String)|
| threasholdType    | It will need other player user "/random 20" command to continue the NPC talk, different type have corresponding attribute bonus for different player. If the threasholType is "选择", the other player should text relate word to the NPC and continue the process.(String)|
| thresholdValue    | The threshold value of the threasholdType, it allows NPCs to speak in two ways, success or fail, if success will jump to the "successJump", otherwise the "failedJump".(int)| 
| failedJump    |  A keyword of the other player random command result is lower than the thresholdValue, it will use the "failedJump" to the next text(String)|
| speaker    | Speaker can overwrite the beginning character name when say the text.(String)|
| condition    | The text need the other player text the specify text to NPC, then the NPC talk can continue.(String)|
| choice1Jump    | The first keyword of the next text need to say when threasholdType is "选择"(String)|
| choice2Jump    | The Second keyword of the next text need to say when threasholdType is "选择"(String)|

### Prerequisites

RandomTalkPlugin assumes all the following prerequisites are met:

* XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
* XIVLauncher is installed to its default directories and configurations.
  * If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
* A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

