using BepInEx;
using BetterEmote.AssetScripts;
using BetterEmote.Patches;
using BetterEmote.Utils;
using EmotesAPI;
using LethalEmotesAPI.ImportV2;
using LethalEmotesAPI.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static UniJSON.JsonFormatter;

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
            SetupBetterEmote(9, "Prisyadka");
            SetupBetterEmote(10, "Sign");


            CustomEmotesAPI.animChanged += BetterEmotesHandler;
        }

        private static void BetterEmotesHandler(string newAnimation, BoneMapper mapper)
        {
            if (!newAnimation.Contains("BetterEmotes__"))
            {
                return;
            }
            if (mapper.prevClipName.Contains("TooManyEmotes__"))
            {
                TooManyEmotesCompat.StopEmote(mapper);
                CustomEmotesAPI.PlayAnimation(newAnimation, mapper);
            }
            else
            {
                if (mapper.local)
                {
                    newAnimation = newAnimation.Split($"BetterEmotes__").Last();
                    InputAction.CallbackContext context = default(InputAction.CallbackContext);
                    mapper.StartCoroutine(PlayBetterEmote(context, newAnimation, mapper));
                }
            }
        }
        internal static IEnumerator PlayBetterEmote(InputAction.CallbackContext context, string newAnimation, BoneMapper mapper)
        {
            mapper.canStop = false;
            yield return new WaitForEndOfFrame();
            EmoteKeybindPatch.onEmoteKeyPerformed(context, (Emote)int.Parse(newAnimation));
            yield return new WaitForEndOfFrame();
            mapper.canStop = true;
        }
        public static void SetupBetterEmote(int num, string displayName)
        {
            CustomEmoteParams param = new CustomEmoteParams();
            param.displayName = displayName;
            param.internalName = $"BetterEmotes__{num}";
            param.rootBonesToIgnore = [HumanBodyBones.Hips];
            param.primaryAnimationClips = [Assets.Load<AnimationClip>($"@CustomEmotesAPI_fineilldoitmyself:assets/fineilldoitmyself/BindPose.anim")];
            EmoteImporter.ImportEmote(param);
            BoneMapper.animClips.Last().Value.ownerPlugin = BetterEmotesPlugin;
        }
    }
}
