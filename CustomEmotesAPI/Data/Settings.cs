using System;
using System.Text;
using BepInEx.Configuration;
using LethalEmotesAPI.Data;
using UnityEngine;
using LethalConfig.ConfigItems;
using LethalConfig;
using LethalEmotesAPI.Utils;
using System.IO;
using System.Linq;

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
        public static EmotesAPIConfigEntries localConfig;
        public static EmotesAPIConfigEntries globalConfig;

        #region Global Entries

        public static ConfigEntry<bool> useGlobalConfig;
        public static ConfigEntry<bool> dontShowDmcaPrompt;
        public static ConfigEntry<string> disallowedModPackOverrideLocations;
        public static ConfigEntry<bool> joinEmoteTutorial;

        #endregion

        #region Local Entries

        public static ConfigEntry<bool> modPackOverride;

        #endregion
        
        #region Common Entries

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
        public static ConfigEntry<bool> ImportTME => GetCurrentConfig().ImportTME;
        public static ConfigEntry<bool> ImportBetterEmotes => GetCurrentConfig().ImportBetterEmotes;
        public static ConfigEntry<bool> NearestEmoteText => GetCurrentConfig().NearestEmoteText;
        public static ConfigEntry<bool> InteractionToolTip => GetCurrentConfig().InteractionToolTip;
        public static ConfigEntry<bool> FillHealthBar => GetCurrentConfig().FillHealthBar;

        #endregion

        public static void RunAll()
        {
            SetProfileName();
            GenerateConfigs();
            LethalConfig();
        }

        internal static GameObject picker;
        
        internal static string profileName;
        
        internal static void SetProfileName()
        {
            profileName = Directory.GetParent(GetBepinexFolder(CustomEmotesAPI.instance.Info.Location))!.Name;
        }
        
        internal static string GetBepinexFolder(string directory)
        {
            string original = directory;
            bool foundIt = false;
            for (int i = 0; i < 20; i++)
            {
                var parent = Directory.GetParent(directory);
                
                if (parent is not null)
                {
                    if (parent.Name.ToLowerInvariant() == "bepinex")
                    {
                        directory = parent.FullName;
                        foundIt = true;
                        break;
                    }
                    directory = parent.FullName;
                }
            }
            return foundIt ? directory : original;
        }
        
        internal static string GetGlobalSettingsDir()
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var saveDir = Path.Combine(userDir, "AppData", "LocalLow", "ZeekerssRBLX", "Lethal Company");
            return Path.Combine(saveDir, "LethalEmotesAPI");
        }
        
        private static void GenerateConfigs()
        {
            modPackOverride = CustomEmotesAPI.instance.Config.Bind<bool>("Local Settings", "Modpack Override", false, "When true, will use local config even when \"Use Global Config\" is set to true. If \"Use Global Config\" is touched by the user, this will always be reset to false.");
            string globalConfigPath = Path.Combine(GetGlobalSettingsDir(), "global.cfg");
            bool globalIsNew = !File.Exists(globalConfigPath);
            
            ConfigFile global = new ConfigFile(globalConfigPath, true, CustomEmotesAPI.instance.Info.Metadata);
            BindGlobalEntries(global);
            
            globalConfig = new EmotesAPIConfigEntries(global, "Global ");
            localConfig = new EmotesAPIConfigEntries(CustomEmotesAPI.instance.Config, "");
            
            if (disallowedModPackOverrideLocations.Value.Split('ඞ').ToList().Contains(profileName))
            {
                modPackOverride.Value = false;
            }
            
            if (globalIsNew)
            {
                globalConfig.CopyFromConfig(localConfig);
            }
        }

        private static void BindGlobalEntries(ConfigFile global)
        {
            useGlobalConfig = global.Bind("Global Settings", "Use Global Config", true, "When true, all EmotesAPI settings will be the same across profiles. When false, each profile will have its own settings.");
            disallowedModPackOverrideLocations = global.Bind("Global No Touch", "Modpack locations disallowing modPackOverride", "", "Locations where users have already opted out of using the modpack specific options");
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(useGlobalConfig, false));
            
            useGlobalConfig.SettingChanged += PermanentEmotingHealthbar_SettingChanged;
            useGlobalConfig.SettingChanged += HideJoinSpots_SettingChanged;
            useGlobalConfig.SettingChanged += UseGlobalConfig_SettingChanged;

            dontShowDmcaPrompt = global.Bind("Global Flags", "Dont Show DMCA Prompt", false,
                "Disables the DMCA prompt from showing in-game. DMCA Prompt automatically shows when we detect OBS or XSplit open");

            joinEmoteTutorial = global.Bind("Global Flags", "Join Emote Tutorial", true, 
                "Guaruntees the join prompt will be in the top left corner");
        }

        private static void UseGlobalConfig_SettingChanged(object sender, EventArgs e)
        {
            Debug.Log($"setting actually changed");
            modPackOverride.Value = false;
            if (!disallowedModPackOverrideLocations.Value.Split('ඞ').ToList().Contains(profileName))
            {
                disallowedModPackOverrideLocations.Value += $"ඞ{profileName}";
            }
        }

        public static EmotesAPIConfigEntries GetCurrentConfig()
        {
            return modPackOverride.Value ? localConfig : useGlobalConfig.Value ? globalConfig : localConfig;
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
