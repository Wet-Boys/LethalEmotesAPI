using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BepInEx.Configuration;
using LethalEmotesAPI.Data;
using LethalEmotesApi.Ui;
using LethalEmotesApi.Ui.Data;
using UnityEngine;
using UnityEngine.Events;
using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;
using LethalConfig;
using LethalEmotesAPI.Utils;
using System.IO;

namespace EmotesAPI
{
    public enum DMCAType
    {
        Normal,
        Friendly,
        Mute,
        AllOff
    }
    public enum RootMotionType
    {
        Normal,
        All,
        None
    }
    public enum ThirdPersonType
    {
        Normal,
        All,
        None
    }
    public static class Settings
    {
        public static ConfigEntry<bool> useGlobalConfig;
        public static EmotesAPIConfigEntries localConfig;
        public static EmotesAPIConfigEntries globalConfig;

        public static ConfigEntry<float> EmotesVolume => GetCurrentConfig().EmotesVolume;
        public static ConfigEntry<bool> HideJoinSpots => GetCurrentConfig().HideJoinSpots;
        public static ConfigEntry<RootMotionType> rootMotionType => GetCurrentConfig().rootMotionType;
        public static ConfigEntry<bool> EmotesAlertEnemies => GetCurrentConfig().EmotesAlertEnemies;
        public static ConfigEntry<DMCAType> DMCAFree => GetCurrentConfig().DMCAFree;
        public static ConfigEntry<ThirdPersonType> thirdPersonType => GetCurrentConfig().thirdPersonType;
        public static ConfigEntry<bool> StopEmoteWhenLockedToStopsEmote => GetCurrentConfig().StopEmoteWhenLockedToStopsEmote;
        public static ConfigEntry<string> EmoteWheelSetDataEntryString => GetCurrentConfig().EmoteWheelSetDataEntryString;
        public static ConfigEntry<string> EmoteWheelSetDisplayDataString => GetCurrentConfig().EmoteWheelSetDisplayDataString;
        public static ConfigEntry<string> RandomEmoteBlacklist => GetCurrentConfig().RandomEmoteBlacklist;
        public static ConfigEntry<string> DisabledEmotes => GetCurrentConfig().DisabledEmotes;
        public static ConfigEntry<bool> PermanentEmotingHealthbar => GetCurrentConfig().PermanentEmotingHealthbar;
        public static ConfigEntry<string> EmoteKeyBinds => GetCurrentConfig().EmoteKeyBinds;
        public static void RunAll()
        {
            GenerateConfigs();
            LethalConfig();
        }

        internal static GameObject picker;

        internal static string GetGlobalSettingsDir()
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var saveDir = Path.Combine(userDir, "AppData", "LocalLow", "ZeekerssRBLX", "Lethal Company");
            return Path.Combine(saveDir, "LethalEmotesAPI");
        }
        
        private static void GenerateConfigs()
        {
            string globalConfigPath = Path.Combine(GetGlobalSettingsDir(), "global.cfg");
            bool globalIsNew = !File.Exists(globalConfigPath);
            ConfigFile global = new ConfigFile(globalConfigPath, true, CustomEmotesAPI.instance.Info.Metadata);
            useGlobalConfig = global.Bind<bool>("Global Settings", "Use Global Config", true, "When true, all EmotesAPI settings will be the same across profiles. When false, each profile will have its own settings.");
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(useGlobalConfig, false));
            useGlobalConfig.SettingChanged += PermanentEmotingHealthbar_SettingChanged;
            useGlobalConfig.SettingChanged += HideJoinSpots_SettingChanged;
            globalConfig = new EmotesAPIConfigEntries(global, "Global ");
            localConfig = new EmotesAPIConfigEntries(CustomEmotesAPI.instance.Config, "");
            if (globalIsNew)
            {
                globalConfig.CopyFromConfig(localConfig);
            }
        }

        public static EmotesAPIConfigEntries GetCurrentConfig()
        {
            return useGlobalConfig.Value ? globalConfig : localConfig;
        }


        internal static void PermanentEmotingHealthbar_SettingChanged(object sender, EventArgs e)
        {
            SetHealthbarRequest();
        }
        internal static void SetHealthbarRequest()
        {
            HealthbarAnimator.permaOn = PermanentEmotingHealthbar.Value;
            HealthbarAnimator.SetHealthbarPosition();
        }

        internal static void HideJoinSpots_SettingChanged(object sender, EventArgs e)
        {
            if (!HideJoinSpots.Value)
            {
                EmoteLocation.ShowAllSpots();
            }
        }

        internal static void PressButton()
        {
            picker.SetActive(false);
            picker.SetActive(true);
            picker.transform.Find("emotepicker").gameObject.SetActive(true);
            picker.transform.SetAsLastSibling();
            picker.GetComponent<Canvas>().sortingOrder = 5;
        }
        internal static void DebugBones(GameObject fab, int spot = 0)
        {
            var meshes = fab.GetComponentsInChildren<SkinnedMeshRenderer>();
            StringBuilder sb = new StringBuilder();
            sb.Append($"rendererererer: {meshes[spot]}\n");
            sb.Append($"bone count: {meshes[spot].bones.Length}\n");
            sb.Append($"mesh count: {meshes.Length}\n");
            sb.Append($"root bone: {meshes[spot].rootBone.name}\n");
            sb.Append($"{fab.ToString()}:\n");
            if (meshes[spot].bones.Length == 0)
            {
                sb.Append("No bones");
            }
            else
            {
                sb.Append("[");
                foreach (var bone in meshes[spot].bones)
                {
                    sb.Append($"'{bone.name}', ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append("]");
            }
            sb.Append("\n\n");
            DebugClass.Log(sb.ToString());
        }
        internal static void LethalConfig()
        {
            var aVeryCoolIconAsset = Assets.Load<Sprite>("lethalconfigicon.png");
            LethalConfigManager.SetModIcon(aVeryCoolIconAsset);
            LethalConfigManager.SetModDescription("API for importing animations to Lethal Company");
            LethalConfigManager.SkipAutoGenFor("No Touch");
        }

    }
}
