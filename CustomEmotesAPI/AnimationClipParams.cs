using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EmotesAPI
{
    public class AnimationClipParams
    {
        public AnimationClip[] animationClip; //list of primary animation clips, one of these will be picked randomly if you include more than one
        public AnimationClip[] secondaryAnimation = null; // list of secondary animation clips, must be the same size as animationClip, will be picked randomly in the same slot (so if we pick animationClip[3] for an animation, we will also pick secondaryAnimation[3])
        public bool looping; //used to specify if audio is looping, you only need to set this if you are importing a single audio file
        public AudioClip[] _primaryAudioClips = null; //primary list of audio clips
        public AudioClip[] _secondaryAudioClips = null; //secondary list of audio clips, if these are specified, the primary clip will never loop and the secondary clip that plays will always loop
        public AudioClip[] _primaryDMCAFreeAudioClips = null; //same as _primaryAudioClips but will be played if DMCA settings allow it (if normal audio clips exist, and dmca clips do not, the dmca clips will simply be silence)
        public AudioClip[] _secondaryDMCAFreeAudioClips = null; //same as _secondaryAudioClips but will be played if DMCA settings allow it
        public HumanBodyBones[] rootBonesToIgnore = null; 
        public HumanBodyBones[] soloBonesToIgnore = null; 
        public bool dimWhenClose = false; // Unused in Lethal Company
        public bool stopWhenMove = false; // If on, will turn off the emote when the player starts moving (very lame, do not use)
        public bool stopWhenAttack = false; // Unused in Lethal Company
        public bool visible = true; // If false, will hide the emote from all normal areas, however it can still be invoked through PlayAnimation, use this for emotes that are only needed in code
        public bool syncAnim = false; // If true, will sync animation between all people emoting
        public bool syncAudio = false; // If true, will sync audio between all people emoting
        public int startPref = -1; // Spot in animationClip array where a BoneMapper will play when there is no other instance of said emote playing -1 is random, -2 is sequential, anything else is what you make it to be
        public int joinPref = -1; // Spot in animationClip array where a BoneMapper will play when there is at least one other instance of said emote playing, -1 is random, -2 is sequential, anything else is what you make it to be
        public JoinSpot[] joinSpots = null; // Array of join spots which will appear when the animation is playing
        public bool useSafePositionReset = false; // Unused in Lethal Company
        public string customName = ""; // Custom name for emote, if not specified, the first emote from animationClip will be used as the name
        public Action<BoneMapper> customPostEventCodeSync = null; // if declared, will fire when an emote plays audio with sync enabled
        public Action<BoneMapper> customPostEventCodeNoSync = null;// if declared, will fire when an emote plays audio with sync disabled
        public LockType lockType = LockType.none; // determines the lock type of your emote, none, headBobbing, lockHead, or rootMotion
        public bool willGetClaimedByDMCA = false; // Lets you mark if your normal set of audio will get claimed by DMCA
        public float audioLevel = .5f; // determines the volume of the emote in terms of alerting enemies, 0 is nothing, 1 is max
        public bool thirdPerson = false;

        public enum LockType
        {
            none,
            headBobbing,
            lockHead,
            rootMotion
        }
    }
}
