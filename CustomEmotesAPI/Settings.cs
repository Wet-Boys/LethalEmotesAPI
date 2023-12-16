using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BepInEx.Configuration;
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
        public static ConfigEntry<bool> DMCAFree;
        //public static ConfigEntry<bool> RemoveAutoWalk;

        public static ConfigEntry<string> emote0;
        public static ConfigEntry<string> emote1;
        public static ConfigEntry<string> emote2;
        public static ConfigEntry<string> emote3;
        public static ConfigEntry<string> emote4;
        public static ConfigEntry<string> emote5;
        public static ConfigEntry<string> emote6;
        public static ConfigEntry<string> emote7;
        public static ConfigEntry<string> emote8;
        public static ConfigEntry<string> emote9;
        public static ConfigEntry<string> emote10;
        public static ConfigEntry<string> emote11;
        public static ConfigEntry<string> emote12;
        public static ConfigEntry<string> emote13;
        public static ConfigEntry<string> emote14;
        public static ConfigEntry<string> emote15;
        public static ConfigEntry<string> emote16;
        public static ConfigEntry<string> emote17;
        public static ConfigEntry<string> emote18;
        public static ConfigEntry<string> emote19;
        public static ConfigEntry<string> emote20;
        public static ConfigEntry<string> emote21;
        public static ConfigEntry<string> emote22;
        public static ConfigEntry<string> emote23;



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
            EmotesAlertEnemies = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Emotes Alert Enemies", false, "If turned on, emotes will alert enemies like other sound sources.");
            EmotesVolume = CustomEmotesAPI.instance.Config.Bind<float>("Controls", "Emotes Volume", 50, "Emotes \"Should\" be controlled by Volume SFX as well, but this is a seperate slider if you want a different audio balance.");
            DMCAFree = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "DMCA Free Songs", false, "If turned on, emotes will either use their DMCA free version or no audio at all.");

            emote0 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes0", "none", "Messing with this here is not reccomended, like at all");
            emote1 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes1", "none", "Messing with this here is not reccomended, like at all");
            emote2 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes2", "none", "Messing with this here is not reccomended, like at all");
            emote3 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes3", "none", "Messing with this here is not reccomended, like at all");
            emote4 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes4", "none", "Messing with this here is not reccomended, like at all");
            emote5 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes5", "none", "Messing with this here is not reccomended, like at all");
            emote6 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes6", "none", "Messing with this here is not reccomended, like at all");
            emote7 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes7", "none", "Messing with this here is not reccomended, like at all");
            emote8 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes8", "none", "Messing with this here is not reccomended, like at all");
            emote9 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes9", "none", "Messing with this here is not reccomended, like at all");
            emote10 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes10", "none", "Messing with this here is not reccomended, like at all");
            emote11 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes11", "none", "Messing with this here is not reccomended, like at all");
            emote12 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes12", "none", "Messing with this here is not reccomended, like at all");
            emote13 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes13", "none", "Messing with this here is not reccomended, like at all");
            emote14 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes14", "none", "Messing with this here is not reccomended, like at all");
            emote15 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes15", "none", "Messing with this here is not reccomended, like at all");
            emote16 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes16", "none", "Messing with this here is not reccomended, like at all");
            emote17 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes17", "none", "Messing with this here is not reccomended, like at all");
            emote18 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes18", "none", "Messing with this here is not reccomended, like at all");
            emote19 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes19", "none", "Messing with this here is not reccomended, like at all");
            emote20 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes20", "none", "Messing with this here is not reccomended, like at all");
            emote21 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes21", "none", "Messing with this here is not reccomended, like at all");
            emote22 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes22", "none", "Messing with this here is not reccomended, like at all");
            emote23 = CustomEmotesAPI.instance.Config.Bind<string>("Data", "Bind for emotes23", "none", "Messing with this here is not reccomended, like at all");

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
