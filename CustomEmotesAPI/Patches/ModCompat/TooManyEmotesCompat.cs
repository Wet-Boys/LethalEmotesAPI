using BepInEx;
using EmotesAPI;
using LethalEmotesAPI.ImportV2;
using LethalEmotesAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TooManyEmotes;
using UnityEngine;

namespace LethalEmotesAPI.Patches.ModCompat
{
    public class TooManyEmotesCompat
    {
        public static BepInPlugin TMEPlugin = Assembly.GetAssembly(typeof(EmotesManager)).GetBepInPlugin();

        public static void RegisterAllTooManyEmotesEmotes()
        {
            foreach (var item in EmotesManager.allUnlockableEmotesDict)
            {
                CustomEmoteParams param = new CustomEmoteParams();
                param.displayName = item.Value.displayName;
                param.internalName = $"TooManyEmotes__{item.Key}";
                param.nonAnimatingEmote = true;
                param.primaryAnimationClips = [item.Value.animationClip];
                EmoteImporter.ImportEmote(param);
                BoneMapper.animClips.Last().Value.ownerPlugin = TMEPlugin;
            }

            CustomEmotesAPI.animChanged += TooManyEmotesHandler;
        }

        private static void TooManyEmotesHandler(string newAnimation, BoneMapper mapper)
        {
            if (!newAnimation.Contains("TooManyEmotes__"))
            {
                return;
            }
            newAnimation = newAnimation.Split($"TooManyEmotes__").Last();
            mapper.PlayAnim("none", -1);
            mapper.mapperBody.GetComponent<EmoteController>().PerformEmote(EmotesManager.allUnlockableEmotesDict[newAnimation]);
        }
    }
}
