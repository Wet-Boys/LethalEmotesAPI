using BepInEx.Configuration;
using EmotesAPI;
using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;
using LethalConfig;
using LethalEmotesApi.Ui.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalEmotesAPI.Data
{
    public class EmotesAPIConfigEntries
    {
        public ConfigEntry<float> EmotesVolume;
        public ConfigEntry<bool> HideJoinSpots;
        public ConfigEntry<RootMotionType> rootMotionType;
        public ConfigEntry<bool> EmotesAlertEnemies;
        public ConfigEntry<DMCAType> DMCAFree;
        public ConfigEntry<ThirdPersonType> thirdPersonType;
        public ConfigEntry<bool> StopEmoteWhenLockedToStopsEmote;
        public ConfigEntry<string> EmoteWheelSetDataEntryString;
        public ConfigEntry<string> EmoteWheelSetDisplayDataString;
        public ConfigEntry<string> RandomEmoteBlacklist;
        public ConfigEntry<string> DisabledEmotes;
        public ConfigEntry<bool> PermanentEmotingHealthbar;
        public ConfigEntry<string> EmoteKeyBinds;
        public ConfigEntry<bool> ImportTME;
        public ConfigEntry<bool> ImportBetterEmotes;
        public ConfigEntry<bool> NearestEmoteText;
        public ConfigEntry<bool> InteractionToolTip;
        public ConfigEntry<bool> FillHealthBar;
        private ConfigFile configFile;
        private string sectionPrefix;
        public EmotesAPIConfigEntries(ConfigFile file, string sectionPrefix)
        {
            HideJoinSpots = file.Bind<bool>(sectionPrefix + "Misc", "Hide Join Spots When Animating", false, "Hides all join spots when you are performing an animation, this loses some visual clarity but offers a more C I N E M A T I C experience");
            rootMotionType = file.Bind<RootMotionType>(sectionPrefix + "Controls", "Camera Lock Settings", RootMotionType.Normal, "Switch head locking between all emotes, no emotes, or let each emote decide.");
            EmotesAlertEnemies = file.Bind<bool>(sectionPrefix + "Misc", "Emotes Alert Enemies", true, "If turned on, emotes will alert enemies like other sound sources.");
            EmotesVolume = file.Bind<float>(sectionPrefix + "Controls", "Emotes Volume", 50, "Emotes \"Should\" be controlled by Volume SFX as well, but this is a separate slider if you want a different audio balance.");
            DMCAFree = file.Bind<DMCAType>(sectionPrefix + "Misc", "DMCA Free Songs", DMCAType.Normal, "0: All songs will be normal. 1: All songs will use normal/DMCA friendly depending on the import settings. 2: All songs will be muted if DMCA is listed. 3: All songs will use DMCA friendly versions or none at all");
            EmoteWheelSetDataEntryString = file.Bind(sectionPrefix + "No Touch", "Emote Wheel Set Data", EmoteWheelSetData.Default().ToJson(), "Json data of emote wheel");
            EmoteWheelSetDisplayDataString = file.Bind(sectionPrefix + "No Touch", "Emote Wheel Set Display Data", new EmoteWheelSetDisplayData().ToJson(), "Json data of emote wheel display");
            RandomEmoteBlacklist = file.Bind<string>(sectionPrefix + "No Touch", "Blacklisted emotes", "none", "Emotes which will not show up when pressing the random emote key, probably don't want to touch this here");
            DisabledEmotes = file.Bind<string>(sectionPrefix + "No Touch", "Disabled Emotes", "", "Emotes on this list will not actually play when called to play, probably don't want to touch this here");
            thirdPersonType = file.Bind<ThirdPersonType>(sectionPrefix + "Controls", "Third Person Settings", ThirdPersonType.Normal, "Switch third person settings between emote decides, all on, or all off");
            StopEmoteWhenLockedToStopsEmote = file.Bind<bool>(sectionPrefix + "Misc", "Stop Emote When Locked Player Stops Emote", true, "If you are locked to a player for an emote (determined by emote mods themselves), you will stop emoting and unlock yourself if the other person stops emoting");
            PermanentEmotingHealthbar = file.Bind<bool>(sectionPrefix + "Misc", "Permanent Healthbar Animation", false, "Keeps the fun lil guy in the top left animating at all times");
            EmoteKeyBinds = file.Bind<string>(sectionPrefix + "No Touch", "Emote Keybinds", "{}", "Emote Keybinds, don't touch it here, change in the UI");
            ImportTME = file.Bind<bool>(sectionPrefix + "Misc", "Import TooManyEmotes", true, "If turned on, emotes from TooManyEmotes will also be available in LethalEmotesAPI's menus");
            ImportBetterEmotes = file.Bind<bool>(sectionPrefix + "Misc", "Import BetterEmotes", true, "If turned on, emotes from BetterEmotes will also be available in LethalEmotesAPI's menus");
            NearestEmoteText = file.Bind<bool>(sectionPrefix + "Misc", "Nearest Emote Text", false, "If turned on, will display the nearest joinable emote in the top left corner while not emoting");
            InteractionToolTip = file.Bind<bool>(sectionPrefix + "Misc", "Interaction Tooltip", false, "If turned on, will display an interaction tooltip when looking at joinable players");
            FillHealthBar = file.Bind<bool>(sectionPrefix + "Misc", "Fill Health Bar", false, "If turned on, will display the health value as a fill value, instead of just alpha levels.");

            HideJoinSpots.SettingChanged += Settings.HideJoinSpots_SettingChanged;
            PermanentEmotingHealthbar.SettingChanged += Settings.PermanentEmotingHealthbar_SettingChanged;
            configFile = file;
            this.sectionPrefix = sectionPrefix;

            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<RootMotionType>(rootMotionType, false));
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(EmotesVolume, new FloatSliderOptions { Min = 0, Max = 100, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<DMCAType>(DMCAFree, false));
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<ThirdPersonType>(thirdPersonType, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(EmotesAlertEnemies, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(HideJoinSpots, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(StopEmoteWhenLockedToStopsEmote, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(PermanentEmotingHealthbar, false));
        }
        public void CopyFromConfig(EmotesAPIConfigEntries sourceConfig)
        {
            foreach (var item in sourceConfig.configFile.Keys)
            {
                var definition = new ConfigDefinition(sectionPrefix + item.Section, item.Key);
                if (configFile.ContainsKey(definition))
                {
                    configFile[definition].SetSerializedValue(sourceConfig.configFile[item].GetSerializedValue());
                }
            }
        }
    }
}
