using BepInEx;
using BetterEmote.AssetScripts;
using BetterEmote.Utils;
using EmotesAPI;
using LethalEmotesAPI.ImportV2;
using LethalEmotesAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.InputSystem;

namespace LethalEmotesAPI.Patches.ModCompat
{
    public class BetterEmotesCompat
    {
        public static BepInPlugin BetterEmotesPlugin = Assembly.GetAssembly(typeof(SignUI)).GetBepInPlugin();

        public static void RegisterAllTooManyEmotesEmotes()
        {
            SetupBetterEmote(3, "Middle Finger");
            SetupBetterEmote(4, "Clap");
            SetupBetterEmote(5, "Shy");
            SetupBetterEmote(6, "The Griddy");
            SetupBetterEmote(7, "Twerk");
            SetupBetterEmote(8, "Salute");
            SetupBetterEmote(9, "Squat Kick");
            SetupBetterEmote(0, "Custom Sign");


            CustomEmotesAPI.animChanged += TooManyEmotesHandler;
        }

        private static void TooManyEmotesHandler(string newAnimation, BoneMapper mapper)
        {
            if (!newAnimation.Contains("BetterEmotes__"))
            {
                return;
            }
            //mapper.PlayAnim("none", -1);
            if (mapper.local)
            {
                newAnimation = newAnimation.Split($"BetterEmotes__").Last();
                InputAction.CallbackContext context = default(InputAction.CallbackContext);
                mapper.playerController.PerformEmote(context, int.Parse(newAnimation));
            }
        }
        public static void SetupBetterEmote(int num, string displayName)
        {
            CustomEmoteParams param = new CustomEmoteParams();
            param.displayName = displayName;
            param.internalName = $"BetterEmotes__{num}";
            param.nonAnimatingEmote = true;
            EmoteImporter.ImportEmote(param);
            BoneMapper.animClips.Last().Value.ownerPlugin = BetterEmotesPlugin;
        }
    }
}
