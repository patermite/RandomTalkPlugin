using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dalamud.Logging.Internal;
namespace RandomTalkPlugin.Lottery
{
    public class LotterydSaver
    {
        private Dictionary<int, (string, string)> giftDict = new Dictionary<int, (string,string)> { };
        private Dictionary<(string, string),  string> giftDestinationDict = new Dictionary<(string, string), string> { };
        private string savePath = "";
        private string giftFileName = "GiftList";
        public ModuleLog moduleLog { get; set; }
        public void Init (IDalamudPluginInterface pluginInterface)
        {
            savePath = pluginInterface.ConfigDirectory.FullName;
        }

        public void LoadPlayerGift(IDalamudPluginInterface pluginInterface)
        {
            if (!File.Exists(pluginInterface.ConfigDirectory.FullName + $"\\" + giftFileName + ".json")) return;
            string content = File.ReadAllText(Path.Join(pluginInterface.ConfigDirectory.FullName, giftFileName + ".json"));
            var playerGift = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            if (playerGift == null) return;
            foreach (var (key, value) in playerGift)
            {
                SetGift(key, value);
            }
            return;
        }
        public (string, string) GetGift(int number, string name)
        {
            if (giftDict.TryGetValue(number, out (string,string) value))
            {

                giftDestinationDict[value] = name;
                giftDict.Remove(number);
                return value;
            }
            return ("", "");
        }

        public void SetGift(string name, string giftName)
        {
            int i = 1;
            for (i=1; giftDict.TryGetValue(giftDict.Count + i,out (string, string) value);i++) {

            }
            giftDict[giftDict.Count + i] = (name, giftName);

            var shuffledNumbers = Enumerable.Range(1, giftDict.Count).OrderBy(x => Guid.NewGuid());
            var valueList = giftDict.Values.ToList();
            moduleLog.Information("[setgift] {0} {1}", shuffledNumbers, valueList);
            var tempDict = shuffledNumbers.Zip(valueList, (key, value) => new { key, value })
                                          .ToDictionary(item => item.key, item => item.value);
            giftDict = tempDict;
        }
        public Dictionary<int, (string, string)> GetGiftDict() { return giftDict; }

        public Dictionary<(string, string), string> GetDestinationGiftDict() { return giftDestinationDict; }

        public void ExportToCsv(IChatGui ChatGui)
        {
            var csv = new StringBuilder();

            foreach (var (key, value) in giftDestinationDict)
            {
                csv.AppendLine($"{key},{value}");
            }
            try
            {
                string file = Path.Join(savePath, "export.csv");
                if (File.Exists(file))
                    File.Delete(file);
                File.WriteAllText(file, csv.ToString());
                ChatGui.Print($"Saved the file to: {Path.Join(savePath, "export.csv")}");
            }
            catch (Exception e)
            {
                ChatGui.PrintError($"Could not save the file! {Path.Join(savePath, "export.csv")}");
                ChatGui.PrintError(e.Message);
            }
        }


    }
    
}
