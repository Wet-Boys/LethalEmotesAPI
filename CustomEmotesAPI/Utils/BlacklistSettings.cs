using BepInEx.Configuration;
using EmotesAPI;
using System.Collections.Generic;
using System.Text;

namespace LethalEmotesAPI.Utils
{
    public class BlacklistSettings
    {
        public static List<string> emotesExcludedFromRandom = [];

        public static void AddToExcludeList(string emoteName)
        {
            emoteName = BoneMapper.GetRealAnimationName(emoteName);
            CustomEmotesAPI.randomClipList.Remove(emoteName);
            if (emotesExcludedFromRandom.Contains(emoteName))
                return;

            emotesExcludedFromRandom.Add(emoteName);
            SaveExcludeListToBepinSex(Settings.RandomEmoteBlacklist);
        }

        public static void RemoveFromExcludeList(string emoteName)
        {
            emoteName = BoneMapper.GetRealAnimationName(emoteName);
            emotesExcludedFromRandom.Remove(emoteName);
            if (CustomEmotesAPI.randomClipList.Contains(emoteName))
                return;

            CustomEmotesAPI.randomClipList.Add(emoteName);
            SaveExcludeListToBepinSex(Settings.RandomEmoteBlacklist);
        }

        public static void LoadExcludeListFromBepinSex(ConfigEntry<string> list)
        {
            string csvList = list.Value;
            foreach (var item in csvList.Split('ඞ'))
            {
                AddToExcludeList(item);
            }
        }

        public static void SaveExcludeListToBepinSex(ConfigEntry<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in emotesExcludedFromRandom)
            {
                sb.Append($"{item}ඞ");
            }
            if (sb.ToString().EndsWith('ඞ'))
            {
                sb.Remove(sb.Length - 1, 1);
            }
            list.Value = sb.ToString();
        }
    }
}
