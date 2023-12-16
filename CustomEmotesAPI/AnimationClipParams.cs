using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EmotesAPI
{
    public class AnimationClipParams
    {
        public AnimationClip[] animationClip;
        public bool looping;
        public AudioClip[] _primaryAudioClips = null;
        public AudioClip[] _secondaryAudioClips = null;
        public HumanBodyBones[] rootBonesToIgnore = null;
        public HumanBodyBones[] soloBonesToIgnore = null;
        public AnimationClip[] secondaryAnimation = null;
        public bool dimWhenClose = false;
        public bool stopWhenMove = false;
        public bool stopWhenAttack = false;
        public bool visible = true;
        public bool syncAnim = false;
        public bool syncAudio = false;
        public int startPref = -1;
        public int joinPref = -1;
        public JoinSpot[] joinSpots = null;
        public bool useSafePositionReset = false;
        public string customName = "";
        public Action<BoneMapper> customPostEventCodeSync = null;
        public Action<BoneMapper> customPostEventCodeNoSync = null;
        public LockType lockType = LockType.none;

        public enum LockType
        {
            none,
            headBobbing,
            lockHead,
            rootMotion
        }
    }
}
