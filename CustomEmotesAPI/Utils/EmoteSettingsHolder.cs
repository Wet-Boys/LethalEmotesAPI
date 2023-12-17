using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static EmotesAPI.AnimationClipParams;

namespace LethalEmotesAPI.Utils
{
    public class EmoteSettingsHolder
    {
        public static List<EmoteSettingsHolder> emoteSettingsHolders = new List<EmoteSettingsHolder>();

        public string emoteName;
        public LockType lockType;
        public bool inRandomPool;

        public static void LoadRandomPoolSettings()
        {
            foreach (var settingHolder in emoteSettingsHolders)
            {
                if (!settingHolder.inRandomPool)
                {
                    CustomEmotesAPI.randomClipList.Remove(settingHolder.emoteName);
                }
            }
        }
        public static void LoadLockTypeSettings()
        {
            foreach (var settingHolder in emoteSettingsHolders)
            {
                if (BoneMapper.animClips.ContainsKey(settingHolder.emoteName))
                {
                    CustomAnimationClip clip = BoneMapper.animClips[settingHolder.emoteName];
                    clip.lockType = settingHolder.lockType;
                }
            }
        }
    }
}
