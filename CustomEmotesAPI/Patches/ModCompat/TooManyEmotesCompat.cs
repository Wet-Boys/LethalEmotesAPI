using BepInEx;
using BetterEmote.Utils;
using EmotesAPI;
using GameNetcodeStuff;
using LethalEmotesAPI.ImportV2;
using LethalEmotesAPI.Utils;
using MonoMod.RuntimeDetour;
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
                param.primaryAnimationClips = [item.Value.animationClip];
                if (!item.Value.animationClip.isLooping)
                {
                    param.secondaryAnimationClips = [Assets.Load<AnimationClip>($"@CustomEmotesAPI_fineilldoitmyself:assets/fineilldoitmyself/BindPose.anim")];
                }
                param.visible = false;
                param.rootBonesToIgnore = [HumanBodyBones.Hips];
                //param.visible = SessionManager.IsEmoteUnlocked(item.Value);
                EmoteImporter.ImportEmote(param);
                BoneMapper.animClips.Last().Value.ownerPlugin = TMEPlugin;
            }

            //CustomEmotesAPI.instance.SetupHook(typeof(SessionManager), typeof(TooManyEmotesCompat), "UnlockEmoteLocal", BindingFlags.Public, nameof(UnlockEmoteLocal), UnlockEmoteLocalHook, true, typeof(int), typeof(string));
            CustomEmotesAPI.animChanged += TooManyEmotesHandler;
        }

        private static void TooManyEmotesHandler(string newAnimation, BoneMapper mapper)
        {
            if (!newAnimation.Contains("TooManyEmotes__"))
            {
                return;
            }
            if (mapper.prevClipName.Contains("BetterEmotes__"))
            {
                CustomEmotesAPI.PlayAnimation(newAnimation, mapper);
            }
            else
            {
                newAnimation = newAnimation.Split($"TooManyEmotes__").Last();
                foreach (var item in EmoteController.allEmoteControllers.Values)
                {
                    if (item.IsPerformingCustomEmote() && item.performingEmote == EmotesManager.allUnlockableEmotesDict[newAnimation] && item.gameObject != mapper.mapperBody)
                    {
                        mapper.mapperBody.GetComponent<EmoteControllerPlayer>().TrySyncingEmoteWithEmoteController(item);
                        return;
                    }
                }
                if (SessionManager.IsEmoteUnlocked(EmotesManager.allUnlockableEmotesDict[newAnimation].emoteName))
                {
                    mapper.mapperBody.GetComponent<EmoteController>().PerformEmote(EmotesManager.allUnlockableEmotesDict[newAnimation]);
                }
            }
        }
        internal static void StopEmote(BoneMapper mapper)
        {
            mapper.mapperBody.GetComponent<EmoteController>().StopPerformingEmote();
        }
        public static void ReloadTooManyEmotesVisibility()
        {
            if (EmotesAPI.Settings.ImportTME.Value)
            {
                foreach (var item in EmotesManager.allUnlockableEmotesDict)
                {
                    if (item.Value.emoteSyncGroup is not null)
                    {
                        if (item.Value.emoteSyncGroup.First() != item.Value)
                        {
                            continue;
                        }
                    }
                    bool needToUnlock = SessionManager.IsEmoteUnlocked(item.Value.emoteName);
                    string emoteName = $"{CustomEmotesAPI.PluginGUID}__TooManyEmotes__{item.Key}";
                    CustomEmotesAPI.randomClipList.Remove(emoteName);
                    if (needToUnlock && !BlacklistSettings.emotesExcludedFromRandom.Contains(emoteName))
                    {
                        CustomEmotesAPI.randomClipList.Add(emoteName);
                    }
                    BoneMapper.animClips[emoteName].visibility = needToUnlock;
                }
            }
        }
    }
}
