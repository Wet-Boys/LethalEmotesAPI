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
        public static void ImportEmote(AnimationClipParams animationClipParams)
        {
            if (animationClipParams.customName == "")
            {
                animationClipParams.customName = animationClipParams.animationClip[0].name;
            }
            animationClipParams.customName = $"{animationClipParams.OwnerPlugin.GUID}__{animationClipParams.customName}";
            if (BoneMapper.animClips.ContainsKey(animationClipParams.customName))
            {
                Debug.Log($"EmotesError: [{animationClipParams.customName}] is already defined as a custom emote but is trying to be added. Skipping");
                return;
            }
            if (!animationClipParams.animationClip[0].isHumanMotion)
            {
                Debug.Log($"EmotesError: [{animationClipParams.customName}] is not a humanoid animation!");
                return;
            }
            if (animationClipParams.rootBonesToIgnore == null)
                animationClipParams.rootBonesToIgnore = new HumanBodyBones[0];
            if (animationClipParams.soloBonesToIgnore == null)
                animationClipParams.soloBonesToIgnore = new HumanBodyBones[0];


            if (animationClipParams._primaryAudioClips == null)
                animationClipParams._primaryAudioClips = new AudioClip[] { null };
            if (animationClipParams._secondaryAudioClips == null)
                animationClipParams._secondaryAudioClips = new AudioClip[] { null };
            if (animationClipParams._primaryDMCAFreeAudioClips == null)
                animationClipParams._primaryDMCAFreeAudioClips = new AudioClip[] { null };
            if (animationClipParams._secondaryDMCAFreeAudioClips == null)
                animationClipParams._secondaryDMCAFreeAudioClips = new AudioClip[] { null };

            List<AudioClip> testClipList = new List<AudioClip>(animationClipParams._primaryDMCAFreeAudioClips);
            while (testClipList.Count != animationClipParams._primaryAudioClips.Length)
            {
                testClipList.Add(null);
            }
            animationClipParams._primaryDMCAFreeAudioClips = testClipList.ToArray();

            testClipList = new List<AudioClip>(animationClipParams._secondaryDMCAFreeAudioClips);
            while (testClipList.Count != animationClipParams._secondaryAudioClips.Length)
            {
                testClipList.Add(null);
            }
            animationClipParams._secondaryDMCAFreeAudioClips = testClipList.ToArray();

            if (animationClipParams.joinSpots == null)
                animationClipParams.joinSpots = new JoinSpot[0];
            CustomAnimationClip clip = new CustomAnimationClip(animationClipParams.animationClip, animationClipParams.looping, animationClipParams._primaryAudioClips, animationClipParams._secondaryAudioClips, animationClipParams.rootBonesToIgnore, animationClipParams.soloBonesToIgnore, animationClipParams.secondaryAnimation, animationClipParams.dimWhenClose, animationClipParams.stopWhenMove, animationClipParams.stopWhenAttack, animationClipParams.visible, animationClipParams.syncAnim, animationClipParams.syncAudio, animationClipParams.startPref, animationClipParams.joinPref, animationClipParams.joinSpots, animationClipParams.useSafePositionReset, animationClipParams.customName, animationClipParams.customPostEventCodeSync, animationClipParams.customPostEventCodeNoSync, animationClipParams.lockType, animationClipParams._primaryDMCAFreeAudioClips, animationClipParams._secondaryDMCAFreeAudioClips, animationClipParams.willGetClaimedByDMCA, animationClipParams.audioLevel, animationClipParams.thirdPerson, animationClipParams.displayName, animationClipParams.OwnerPlugin, animationClipParams.useLocalTransforms, true);
            if (animationClipParams.visible)
            {
                if (!BlacklistSettings.emotesExcludedFromRandom.Contains(animationClipParams.customName))
                {
                    CustomEmotesAPI.randomClipList.Add(animationClipParams.customName);
                }
            }
            BoneMapper.animClips.Add(animationClipParams.customName, clip);

        }
    }
}
