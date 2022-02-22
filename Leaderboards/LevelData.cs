﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Levante.Leaderboards
{
    public class LevelData
    {
        public static readonly string FilePathS15 = @"Data/S15/levelData.json";
        public static readonly string FilePath = @"Data/S16/levelData.json";

        [JsonProperty("LevelDataEntries")]
        public List<LevelDataEntry> LevelDataEntries { get; set; } = new List<LevelDataEntry>();

        public partial class LevelDataEntry : LeaderboardEntry
        {
            [JsonProperty("LastLoggedLevel")]
            public int LastLoggedLevel { get; set; } = -1;

            [JsonProperty("UniqueBungieName")]
            public string UniqueBungieName { get; set; } = "Guardian#0000";
        }

        public List<LevelDataEntry> GetSortedLevelData()
        {
            QuickSort(0, LevelDataEntries.Count - 1);
            return LevelDataEntries;
        }

        private void QuickSort(int Start, int End)
        {
            if (Start < End)
            {
                int partIndex = Partition(Start, End);

                QuickSort(Start, partIndex - 1);
                QuickSort(partIndex + 1, End);
            }
        }

        private int Partition(int Start, int End)
        {
            int Center = LevelDataEntries[End].LastLoggedLevel;

            int i = Start - 1;
            for (int j = Start; j < End; j++)
            {
                if (LevelDataEntries[j].LastLoggedLevel >= Center)
                {
                    i++;
                    var temp1 = LevelDataEntries[i];
                    LevelDataEntries[i] = LevelDataEntries[j];
                    LevelDataEntries[j] = temp1;
                }
            }

            var temp = LevelDataEntries[i + 1];
            LevelDataEntries[i + 1] = LevelDataEntries[End];
            LevelDataEntries[End] = temp;

            return i + 1;
        }

        #region JSONFileHandling

        public void UpdateEntriesConfig()
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }

        public static void AddEntryToConfig(int Level, string BungieName)
        {
            LevelDataEntry lde = new LevelDataEntry()
            {
                LastLoggedLevel = Level,
                UniqueBungieName = BungieName
            };
            string json = File.ReadAllText(FilePath);
            LevelData ld = JsonConvert.DeserializeObject<LevelData>(json);

            ld.LevelDataEntries.Add(lde);
            string output = JsonConvert.SerializeObject(ld, Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }

        public static void AddEntryToConfig(LevelDataEntry lde)
        {
            string json = File.ReadAllText(FilePath);
            LevelData ld = JsonConvert.DeserializeObject<LevelData>(json);

            ld.LevelDataEntries.Add(lde);
            string output = JsonConvert.SerializeObject(ld, Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }

        public static void DeleteEntryFromConfig(string BungieName)
        {
            string json = File.ReadAllText(FilePath);
            LevelData ld = JsonConvert.DeserializeObject<LevelData>(json);
            for (int i = 0; i < ld.LevelDataEntries.Count; i++)
                if (ld.LevelDataEntries[i].UniqueBungieName.Equals(BungieName))
                    ld.LevelDataEntries.RemoveAt(i);
            string output = JsonConvert.SerializeObject(ld, Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }

        public static bool IsExistingLinkedEntry(string BungieName)
        {
            string json = File.ReadAllText(FilePath);
            LevelData ld = JsonConvert.DeserializeObject<LevelData>(json);
            foreach (LevelDataEntry lde in ld.LevelDataEntries)
                if (lde.UniqueBungieName.Equals(BungieName))
                    return true;
            return false;
        }

        #endregion
    }
}
