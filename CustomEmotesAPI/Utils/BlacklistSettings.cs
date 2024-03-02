using BepInEx.Configuration;
using EmotesAPI;
using System.Collections.Generic;
using System.Text;

namespace LethalEmotesAPI.Utils
{
    public class BlacklistSettings
    {
        public static List<string> emotesExcludedFromRandom = [];
        public static List<string> emotesDisabled = [];

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
            RemoveFromDisabledList(emoteName);
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









        //Disable List
        public static void LoadDisabledListFromBepinSex(ConfigEntry<string> list)
        {
            string csvList = list.Value;
            foreach (var item in csvList.Split('ඞ'))
            {
                AddToDisabledList(item);
            }
        }
        public static void AddToDisabledList(string emoteName)
        {
            emoteName = BoneMapper.GetRealAnimationName(emoteName);
            DebugClass.Log($"excluding {emoteName} before we disable it");
            AddToExcludeList(emoteName);
            if (emotesDisabled.Contains(emoteName))
                return;

            emotesDisabled.Add(emoteName);
            SaveDisabledListToBepinSex(Settings.DisabledEmotes);
        }

        public static void RemoveFromDisabledList(string emoteName)
        {
            emoteName = BoneMapper.GetRealAnimationName(emoteName);
            emotesDisabled.Remove(emoteName);
            SaveDisabledListToBepinSex(Settings.DisabledEmotes);
        }
        public static void SaveDisabledListToBepinSex(ConfigEntry<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in emotesDisabled)
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
