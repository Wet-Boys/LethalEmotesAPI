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
        public static ConfigEntry<float> EmotesVolume;
        public static ConfigEntry<bool> HideJoinSpots;
        public static ConfigEntry<RootMotionType> rootMotionType;
        public static ConfigEntry<bool> EmotesAlertEnemies;
        public static ConfigEntry<DMCAType> DMCAFree;
        public static ConfigEntry<ThirdPersonType> thirdPersonType;
        public static ConfigEntry<bool> StopEmoteWhenLockedToStopsEmote;
        //public static ConfigEntry<bool> RemoveAutoWalk;

        public static ConfigEntry<string> EmoteWheelSetDataEntryString;
        public static ConfigEntry<string> RandomEmoteBlacklist;


        public static void RunAll()
        {
            Yes();
            LethalConfig();
        }

        internal static GameObject picker;
        private static void Yes()
        {

            HideJoinSpots = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Hide Join Spots When Animating", false, "Hides all join spots when you are performing an animation, this loses some visual clarity but offers a more C I N E M A T I C experience");
            rootMotionType = CustomEmotesAPI.instance.Config.Bind<RootMotionType>("Controls", "Camera Lock Settings", RootMotionType.Normal, "Switch head locking between all emotes, no emotes, or let each emote decide.");
            EmotesAlertEnemies = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Emotes Alert Enemies", true, "If turned on, emotes will alert enemies like other sound sources.");
            EmotesVolume = CustomEmotesAPI.instance.Config.Bind<float>("Controls", "Emotes Volume", 50, "Emotes \"Should\" be controlled by Volume SFX as well, but this is a seperate slider if you want a different audio balance.");
            DMCAFree = CustomEmotesAPI.instance.Config.Bind<DMCAType>("Misc", "DMCA Free Songs", DMCAType.Normal, "0: All songs will be normal. 1: All songs will use normal/DMCA friendly depending on the import settings. 2: All songs will be muted if DMCA is listed. 3: All songs will use DMCA friendly versions or none at all");
            EmoteWheelSetDataEntryString = CustomEmotesAPI.instance.Config.Bind("No Touch", "Emote Wheel Set Data", EmoteWheelSetData.Default().ToJson(), "Json data of emote wheel");
            RandomEmoteBlacklist = CustomEmotesAPI.instance.Config.Bind<string>("No Touch", "Blacklisted emotes", "none", "Emotes which will not show up when pressing the random emote key, probably don't want to touch this here");
            thirdPersonType = CustomEmotesAPI.instance.Config.Bind<ThirdPersonType>("Controls", "Third Person Settings", ThirdPersonType.Normal, "Switch third person settings between emote decides, all on, or all off");
            StopEmoteWhenLockedToStopsEmote = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Stop Emote When Locked Player Stops Emote", true, "If you are locked to a player for an emote (determined by emote mods themselves), you will stop emoting and unlock yourself if the other person stops emoting");

            HideJoinSpots.SettingChanged += HideJoinSpots_SettingChanged;
        }
        private static void HideJoinSpots_SettingChanged(object sender, EventArgs e)
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


            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<RootMotionType>(rootMotionType, false));
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(EmotesVolume, new FloatSliderOptions { Min = 0, Max = 100, RequiresRestart = false}));
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<DMCAType>(DMCAFree, false));
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<ThirdPersonType>(thirdPersonType, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(EmotesAlertEnemies, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(HideJoinSpots, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(StopEmoteWhenLockedToStopsEmote, false));

        }

    }
}
