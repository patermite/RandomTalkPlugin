using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace RandomTalkPlugin.Lottery
{
    public class LotterydSaver
    {
        private Dictionary<int, (string, string)> giftDict = new Dictionary<int, (string,string)> { };
        private Dictionary<(string, string),  string> giftDestinationDict = new Dictionary<(string, string), string> { };
        private string savePath = "";
        public void Init (DalamudPluginInterface pluginInterface)
        {
            savePath = pluginInterface.ConfigDirectory.FullName;
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
            giftDict[giftDict.Count + 1] = (name, giftName);
            var shuffledNumbers = Enumerable.Range(1, giftDict.Count).OrderBy(x => Guid.NewGuid());
            var valueList = giftDict.Values.ToList();
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
