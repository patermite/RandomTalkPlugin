using System.Collections.Generic;

namespace RandomTalkPlugin.CommandTracker
{
    public struct CharacterDialogue
    {
        public string character;
        public List<Scene> scenes;
    }

    public struct Scene
    {
        public string sceneName;
        public Dictionary<string, TextToSay> textToSay;
    }

    public struct TextToSay
    {
        public string condition;
        public string text;
        public string threasholdTType;
        public int thresholdValue;
        public string successJump;
        public string faliedJump;
        public string choice1Jump;
    }

}
