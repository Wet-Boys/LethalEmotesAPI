using EmotesAPI;
using LethalEmotesAPI.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.ImportV2
{
    public class EmoteImporter
    {
        public static void ImportEmote(CustomEmoteParams animationClipParams)
        {
            if (animationClipParams.internalName == "")
            {
                animationClipParams.internalName = animationClipParams.primaryAnimationClips[0].name;
            }
            animationClipParams.internalName = $"{animationClipParams.OwnerPlugin.GUID}__{animationClipParams.internalName}";
            if (BoneMapper.animClips.ContainsKey(animationClipParams.internalName))
            {
                Debug.Log($"EmotesError: [{animationClipParams.internalName}] is already defined as a custom emote but is trying to be added. Skipping");
                return;
            }
            if (animationClipParams.primaryAnimationClips is null || animationClipParams.primaryAnimationClips.Length == 0)
            {
                animationClipParams.nonAnimatingEmote = true;
            }
            else if (!animationClipParams.primaryAnimationClips[0].isHumanMotion)
            {
                Debug.Log($"EmotesError: [{animationClipParams.internalName}] is not a humanoid animation!");
                return;
            }
            if (animationClipParams.rootBonesToIgnore == null)
                animationClipParams.rootBonesToIgnore = new HumanBodyBones[0];
            if (animationClipParams.soloBonesToIgnore == null)
                animationClipParams.soloBonesToIgnore = new HumanBodyBones[0];


            if (animationClipParams.primaryAudioClips == null)
                animationClipParams.primaryAudioClips = [null];
            if (animationClipParams.secondaryAudioClips == null)
                animationClipParams.secondaryAudioClips = [null];
            if (animationClipParams.primaryDMCAFreeAudioClips == null)
                animationClipParams.primaryDMCAFreeAudioClips = [null];
            if (animationClipParams.secondaryDMCAFreeAudioClips == null)
                animationClipParams.secondaryDMCAFreeAudioClips = [null];

            List<AudioClip> testClipList = new List<AudioClip>(animationClipParams.primaryDMCAFreeAudioClips);
            while (testClipList.Count != animationClipParams.primaryAudioClips.Length)
            {
                testClipList.Add(null);
            }
            animationClipParams.primaryDMCAFreeAudioClips = testClipList.ToArray();

            testClipList = new List<AudioClip>(animationClipParams.secondaryDMCAFreeAudioClips);
            while (testClipList.Count != animationClipParams.secondaryAudioClips.Length)
            {
                testClipList.Add(null);
            }
            animationClipParams.secondaryDMCAFreeAudioClips = testClipList.ToArray();

            if (animationClipParams.joinSpots == null)
                animationClipParams.joinSpots = new JoinSpot[0];
            CustomAnimationClip clip = new CustomAnimationClip(animationClipParams.primaryAnimationClips, animationClipParams.audioLoops, animationClipParams.primaryAudioClips, animationClipParams.secondaryAudioClips, animationClipParams.rootBonesToIgnore, animationClipParams.soloBonesToIgnore, animationClipParams.secondaryAnimationClips, false, animationClipParams.stopWhenMove, false, animationClipParams.visible, animationClipParams.syncAnim, animationClipParams.syncAudio, animationClipParams.startPref, animationClipParams.joinPref, animationClipParams.joinSpots, false, animationClipParams.internalName, animationClipParams.customPostEventCodeSync, animationClipParams.customPostEventCodeNoSync, animationClipParams.lockType, animationClipParams.primaryDMCAFreeAudioClips, animationClipParams.secondaryDMCAFreeAudioClips, animationClipParams.willGetClaimedByDMCA, animationClipParams.audioLevel, animationClipParams.thirdPerson, animationClipParams.displayName, animationClipParams.OwnerPlugin, animationClipParams.useLocalTransforms, true, !animationClipParams.nonAnimatingEmote, animationClipParams.preventMovement);
            if (animationClipParams.visible)
            {
                if (!BlacklistSettings.emotesExcludedFromRandom.Contains(animationClipParams.internalName))
                {
                    CustomEmotesAPI.randomClipList.Add(animationClipParams.internalName);
                }
            }
            BoneMapper.animClips.Add(animationClipParams.internalName, clip);

        }
    }
}
