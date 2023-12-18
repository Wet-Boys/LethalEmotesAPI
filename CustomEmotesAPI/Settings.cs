using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BepInEx.Configuration;
using LethalEmotesApi.Ui;
using LethalEmotesApi.Ui.Data;
using UnityEngine;
using UnityEngine.Events;

namespace EmotesAPI
{
    public static class Settings
    {
        public static ConfigEntry<float> EmotesVolume;
        public static ConfigEntry<bool> HideJoinSpots;
        public static ConfigEntry<bool> AllEmotesHaveRootMotion;
        public static ConfigEntry<bool> NoEmotesHaveRootMotion;
        public static ConfigEntry<bool> AllowHeadBobbing;
        public static ConfigEntry<bool> EmotesAlertEnemies;
        public static ConfigEntry<int> DMCAFree;
        //public static ConfigEntry<bool> RemoveAutoWalk;

        public static ConfigEntry<EmoteWheelSetData> EmoteWheelSetDataEntry;
        public static ConfigEntry<string> RandomEmoteBlacklist;

        //TODO loading a base button
        //public static GameObject NakedButton = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/NakedButton.prefab").WaitForCompletion();
        public static void RunAll()
        {
            //TODO base button
            //UnityEngine.Object.DestroyImmediate(NakedButton.GetComponentInChildren<LanguageTextMeshController>());
            Setup();
            Yes();
        }
        private static void Setup()
        {
            //TODO settings ROO
            //ModSettingsManager.SetModDescription($"Made by Metrosexual Fruitcake#6969\n\nVersion {CustomEmotesAPI.VERSION}");
            //ModSettingsManager.SetModIcon(Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/icon.png"));
        }
        internal static GameObject picker;
        private static void Yes()
        {
            picker = GameObject.Instantiate(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/emotepicker2.prefab"));
            GameObject.DontDestroyOnLoad(picker);
            picker.GetComponent<Canvas>().scaleFactor = 2;
            picker.transform.Find("emotepicker").Find("EmoteContainer").gameObject.AddComponent<ScrollManager>();
            var basedonwhatbasedonthehardwareinside = picker.transform.Find("emotepicker").Find("Wheels").transform.Find("Middle");
            for (int i = 0; i < 8; i++)
            {
                //TODO menu buttons
                //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
                //nut.transform.SetParent(basedonwhatbasedonthehardwareinside.Find($"Button ({i})"));
                //nut.transform.localPosition = new Vector3(-80, -20, 0);
                //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
            }
            basedonwhatbasedonthehardwareinside = picker.transform.Find("emotepicker").Find("Wheels").transform.Find("Left");
            for (int i = 0; i < 8; i++)
            {
                //TODO menu buttons
                //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
                //nut.transform.SetParent(basedonwhatbasedonthehardwareinside.Find($"Button ({i})"));
                //nut.transform.localPosition = new Vector3(-80, -20, 0);
                //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
            }
            basedonwhatbasedonthehardwareinside = picker.transform.Find("emotepicker").Find("Wheels").transform.Find("Right");
            for (int i = 0; i < 8; i++)
            {
                //TODO menu buttons
                //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
                //nut.transform.SetParent(basedonwhatbasedonthehardwareinside.Find($"Button ({i})"));
                //nut.transform.localPosition = new Vector3(-80, -20, 0);
                //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
            }

            HideJoinSpots = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Hide Join Spots When Animating", false, "Hides all join spots when you are performing an animation, this loses some visual clarity but offers a more C I N E M A T I C experience");
            AllEmotesHaveRootMotion = CustomEmotesAPI.instance.Config.Bind<bool>("Controls", "All Emotes Have Root Motion", false, "If turned on, all emotes will have root motion, even if not specified by the emote itself");
            NoEmotesHaveRootMotion = CustomEmotesAPI.instance.Config.Bind<bool>("Controls", "No Emotes Have Root Motion", false, "If turned on, no emotes will have root motion, even if specified by the emote itself");
            AllowHeadBobbing = CustomEmotesAPI.instance.Config.Bind<bool>("Controls", "Allow Head Bobbing Emotes", true, "Some emotes, even if they don't apply root motion, might have head bobbing, this can turn that off.");
            EmotesAlertEnemies = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Emotes Alert Enemies", true, "If turned on, emotes will alert enemies like other sound sources.");
            EmotesVolume = CustomEmotesAPI.instance.Config.Bind<float>("Controls", "Emotes Volume", 50, "Emotes \"Should\" be controlled by Volume SFX as well, but this is a seperate slider if you want a different audio balance.");
            DMCAFree = CustomEmotesAPI.instance.Config.Bind<int>("Misc", "DMCA Free Songs", 0, "0: All songs will be normal. 1: All songs will use normal/DMCA friendly depending on the import settings. 2: All songs will be muted if DMCA is listed. 3: All songs will use DMCA friendly versions or none at all");
            EmoteWheelSetDataEntry = CustomEmotesAPI.instance.Config.Bind("No Touch", "Emote Wheel Set Data", EmoteWheelSetData.Default(), "Json data of emote wheel");
            RandomEmoteBlacklist = CustomEmotesAPI.instance.Config.Bind<string>("No Touch", "Blacklisted emotes", "none", "Emotes which will not show up when pressing the random emote key");

            //TODO settings ROO
            //ModSettingsManager.AddOption(new GenericButtonOption("Customize Emote Wheel", "Controls", PressButton));
            //ModSettingsManager.AddOption(new KeyBindOption(EmoteWheel));
            //ModSettingsManager.AddOption(new KeyBindOption(SetCurrentEmoteToWheel));
            //ModSettingsManager.AddOption(new KeyBindOption(Left));
            //ModSettingsManager.AddOption(new KeyBindOption(Right));
            //ModSettingsManager.AddOption(new SliderOption(EmotesVolume));
            //ModSettingsManager.AddOption(new KeyBindOption(RandomEmote));
            //ModSettingsManager.AddOption(new KeyBindOption(JoinEmote));
            EmotesVolume.SettingChanged += EmotesVolume_SettingChanged;
            HideJoinSpots.SettingChanged += HideJoinSpots_SettingChanged;
            AllEmotesHaveRootMotion.SettingChanged += AllEmotesLock_SettingChanged;
            NoEmotesHaveRootMotion.SettingChanged += NoEmotesLock_SettingChanged;
        }

        private static void AllEmotesLock_SettingChanged(object sender, EventArgs e)
        {
            if (AllEmotesHaveRootMotion.Value)
            {
                NoEmotesHaveRootMotion.Value = false;
            }
        }
        private static void NoEmotesLock_SettingChanged(object sender, EventArgs e)
        {
            if (NoEmotesHaveRootMotion.Value)
            {
                AllEmotesHaveRootMotion.Value = false;
            }
        }
        private static void HideJoinSpots_SettingChanged(object sender, EventArgs e)
        {
            if (!HideJoinSpots.Value)
            {
                EmoteLocation.ShowAllSpots();
            }
        }

        private static void EmotesVolume_SettingChanged(object sender, EventArgs e)
        {
            //TODO audio settings ROO
            //AkSoundEngine.SetRTPCValue("Volume_Emotes", EmotesVolume.Value);
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

    }
}
