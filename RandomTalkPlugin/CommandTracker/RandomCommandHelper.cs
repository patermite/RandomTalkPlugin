using Dalamud.Game.Text.SeStringHandling;
namespace RandomTalkPlugin.CommandTracker
{


    public class RadomCommandHelper
    {
        public (string, string) GetRandomCommandRes(SeString message)
        {
            if (message.TextValue.Contains("掷出了"))
            {
                return (message.TextValue.Split("掷出了")[0], message.TextValue.Split("掷出了")[1].Split("点")[0]);
            }
            return ("", "");
        }
    }

}
