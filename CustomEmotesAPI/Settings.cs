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
        public static ConfigEntry<KeyboardShortcut> EmoteWheel;
        public static ConfigEntry<KeyboardShortcut> Left;
        public static ConfigEntry<KeyboardShortcut> Right;
        public static ConfigEntry<KeyboardShortcut> RandomEmote;
        public static ConfigEntry<KeyboardShortcut> JoinEmote;
        public static ConfigEntry<KeyboardShortcut> SetCurrentEmoteToWheel;
        public static ConfigEntry<float> EmotesVolume;
        public static ConfigEntry<bool> SolSupport;
        public static ConfigEntry<bool> Paladin;
        public static ConfigEntry<bool> Enforcer;
        public static ConfigEntry<bool> Chef;
        public static ConfigEntry<bool> Holomancer;
        public static ConfigEntry<bool> Sett;
        public static ConfigEntry<bool> Tracer;
        public static ConfigEntry<bool> Henry;
        public static ConfigEntry<bool> Katarina;
        public static ConfigEntry<bool> Miner;
        public static ConfigEntry<bool> Phoenix;
        public static ConfigEntry<bool> Scout;
        public static ConfigEntry<bool> Jinx;
        public static ConfigEntry<bool> Soldier;
        public static ConfigEntry<bool> Scavenger;
        public static ConfigEntry<bool> Goku;
        public static ConfigEntry<bool> Trunks;
        public static ConfigEntry<bool> Vegeta;
        public static ConfigEntry<bool> Nemmando;
        public static ConfigEntry<bool> Executioner;
        public static ConfigEntry<bool> Amp;
        public static ConfigEntry<bool> Pathfinder;
        public static ConfigEntry<bool> TF2Medic;
        public static ConfigEntry<bool> Spearman;
        public static ConfigEntry<bool> VoidJailer;
        public static ConfigEntry<bool> Baby;
        public static ConfigEntry<bool> DimmingSpheres;
        public static ConfigEntry<bool> HideJoinSpots;
        //public static ConfigEntry<bool> RemoveAutoWalk;
        public static ConfigEntry<float> DontTouchThis;

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



        //TODO stuff
        //public static GameObject NakedButton = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/NakedButton.prefab").WaitForCompletion();
        public static void RunAll()
        {
            //TODO stuff
            //UnityEngine.Object.DestroyImmediate(NakedButton.GetComponentInChildren<LanguageTextMeshController>());
            Setup();
            Yes();
        }
        private static void Setup()
        {
            //TODO stuff
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
                //TODO stuff
                //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
                //nut.transform.SetParent(basedonwhatbasedonthehardwareinside.Find($"Button ({i})"));
                //nut.transform.localPosition = new Vector3(-80, -20, 0);
                //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
            }
            basedonwhatbasedonthehardwareinside = picker.transform.Find("emotepicker").Find("Wheels").transform.Find("Left");
            for (int i = 0; i < 8; i++)
            {
                //TODO stuff
                //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
                //nut.transform.SetParent(basedonwhatbasedonthehardwareinside.Find($"Button ({i})"));
                //nut.transform.localPosition = new Vector3(-80, -20, 0);
                //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
            }
            basedonwhatbasedonthehardwareinside = picker.transform.Find("emotepicker").Find("Wheels").transform.Find("Right");
            for (int i = 0; i < 8; i++)
            {
                //TODO stuff
                //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
                //nut.transform.SetParent(basedonwhatbasedonthehardwareinside.Find($"Button ({i})"));
                //nut.transform.localPosition = new Vector3(-80, -20, 0);
                //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
            }

            EmoteWheel = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "Emote Wheel", new KeyboardShortcut(KeyCode.C), "Displays the emote wheel");
            Left = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "Cycle Wheel Left", new KeyboardShortcut(KeyCode.Mouse0), "Cycles the emote wheel left");
            Right = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "Cycle Wheel Right", new KeyboardShortcut(KeyCode.Mouse1), "Cycles the emote wheel right");
            EmotesVolume = CustomEmotesAPI.instance.Config.Bind<float>("Controls", "Emotes Volume", 50, "Emotes \"Should\" be controlled by Volume SFX as well, but this is a seperate slider if you want a different audio balance.");
            RandomEmote = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "Play Random Emote", new KeyboardShortcut(KeyCode.G), "Plays a random emote from all available emotes");
            JoinEmote = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "Join Nearest Syncing Emote", new KeyboardShortcut(KeyCode.V), "Picks the nearest player with a syncing emote and joins them");
            SetCurrentEmoteToWheel = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "Bind Current Emote To Wheel", new KeyboardShortcut(KeyCode.B), "Whenever you are performing an emote, if you pull up the emote wheel and press this key while hovering over an emote slot, it will bind the current emote to the selected slot.");
            SolSupport = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Aurelion Sol Support", true, "Support for Aurelion Sol to animate, this will break his model a little bit but if you don't mind that feel free to leave this on");
            Paladin = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Paladin Support", true, "Support for Paladin to animate");
            Enforcer = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Enforcer Support", true, "Support for Enforcer to animate");
            Chef = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Chef Support", true, "Support for Chef to animate");
            Holomancer = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Holomancer Support", false, "Support for Holomancer to animate");
            Sett = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Sett Support", true, "Support for Sett to animate");
            Tracer = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Tracer Support", true, "Support for Tracer to animate");
            Henry = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Henry Support", true, "Support for Henry to animate");
            Katarina = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Katarina Support", true, "Support for Katarina to animate");
            Miner = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Miner Support", true, "Support for Miner to animate");
            Phoenix = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Phoenix Support", true, "Support for Phoenix to animate");
            Scout = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Scout Support", true, "Support for Scout to animate");
            Jinx = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Jinx Support", true, "Support for Jinx to animate");
            Soldier = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Soldier Support", true, "Support for Soldier to animate");
            Scavenger = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Scavenger Support", true, "Support for Scavenger to animate");
            Goku = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Goku Support", true, "Support for Goku to animate");
            Trunks = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Trunks Support", true, "Support for Trunks to animate");
            Vegeta = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Vegeta Support", true, "Support for Vegeta to animate");

            Nemmando = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Nemmando Support", true, "Support for Nemmando to animate");
            Executioner = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Executioner Support", true, "Support for Executioner to animate");
            Amp = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Amp Support", true, "Support for Amp to animate");
            Pathfinder = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Pathfinder Support", true, "Support for Pathfinder to animate");
            TF2Medic = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "TF2Medic Support", true, "Support for TF2Medic to animate");
            Spearman = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Spearman Support", true, "Support for Spearman to animate");
            VoidJailer = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "VoidJailer Support", true, "Support for VoidJailer to animate");
            Baby = CustomEmotesAPI.instance.Config.Bind<bool>("Experimental", "Driver Support", true, "Support for Driver to animate");

            DimmingSpheres = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Dimming Spheres", true, "Turn off music dimming when near emotes that support dimming.");
            HideJoinSpots = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Hide Join Spots When Animating", false, "Hides all join spots when you are performing an animation, this loses some visual clarity but offers a more C I N E M A T I C experience");
            //RemoveAutoWalk = CustomEmotesAPI.instance.Config.Bind<bool>("Misc", "Remove AutoWalk Emotes From Random", true, "Prevents emotes with AutoWalk turned on from appearing with the random button.");

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

            DontTouchThis = CustomEmotesAPI.instance.Config.Bind<float>("Data", "Dont Touch This", 69420, "But like actually dont touch this");

            //TODO stuff
            //ModSettingsManager.AddOption(new GenericButtonOption("Customize Emote Wheel", "Controls", PressButton));
            //ModSettingsManager.AddOption(new KeyBindOption(EmoteWheel));
            //ModSettingsManager.AddOption(new KeyBindOption(SetCurrentEmoteToWheel));
            //ModSettingsManager.AddOption(new KeyBindOption(Left));
            //ModSettingsManager.AddOption(new KeyBindOption(Right));
            //ModSettingsManager.AddOption(new SliderOption(EmotesVolume));
            //ModSettingsManager.AddOption(new KeyBindOption(RandomEmote));
            //ModSettingsManager.AddOption(new KeyBindOption(JoinEmote));
            //ModSettingsManager.AddOption(new CheckBoxOption(SolSupport, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Paladin, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Enforcer, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Chef, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Holomancer, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Sett, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Tracer, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Henry, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Katarina, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Miner, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Phoenix, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Scout, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Jinx, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Soldier, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Scavenger, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Goku, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Vegeta, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Trunks, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Nemmando, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Executioner, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Amp, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(TF2Medic, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Pathfinder, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Spearman, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(VoidJailer, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(Baby, true));
            //ModSettingsManager.AddOption(new CheckBoxOption(DimmingSpheres));
            //ModSettingsManager.AddOption(new CheckBoxOption(HideJoinSpots));
            EmotesVolume.SettingChanged += EmotesVolume_SettingChanged;
            HideJoinSpots.SettingChanged += HideJoinSpots_SettingChanged;
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
            //TODO stuff
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
