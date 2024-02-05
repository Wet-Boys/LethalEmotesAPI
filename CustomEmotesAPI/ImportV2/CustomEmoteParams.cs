using BepInEx;
using LethalEmotesAPI.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using static EmotesAPI.AnimationClipParams;

namespace LethalEmotesAPI.ImportV2
{
    public class CustomEmoteParams
    {
        /// <summary>
        /// List of primary animation clips, one of these will be picked randomly if you include more than one
        /// </summary>
        public AnimationClip[] primaryAnimationClips;

        /// <summary>
        /// List of secondary animation clips, must be the same size as primaryAnimationClips, will be picked randomly in the same slot (so if we pick primaryAnimationClips[3] for an animation, we will also pick secondaryAnimationClips[3])
        /// </summary>
        public AnimationClip[] secondaryAnimationClips = null;

        /// <summary>
        /// Used to specify if audio is looping, you only need to set this if you are importing primary audio files, if you use secondaryAudioClips, this does nothing
        /// </summary>
        public bool audioLoops;

        /// <summary>
        /// Primary list of audio clips
        /// </summary>
        public AudioClip[] primaryAudioClips = null;

        /// <summary>
        /// Secondary list of audio clips, if these are specified, the primary clip will never loop and the secondary clip that plays will always loop
        /// </summary>
        public AudioClip[] secondaryAudioClips = null;

        /// <summary>
        /// Same as primaryAudioClips but will be played if DMCA settings allow it (if normal audio clips exist, and dmca clips do not, the dmca clips will simply be silence)
        /// </summary>
        public AudioClip[] primaryDMCAFreeAudioClips = null;

        /// <summary>
        /// Same as secondaryAudioClips but will be played if DMCA settings allow it
        /// </summary>
        public AudioClip[] secondaryDMCAFreeAudioClips = null;

        /// <summary>
        /// If true, will turn off the emote when the player starts moving
        /// </summary>
        public bool stopWhenMove = false;

        /// <summary>
        /// If false, will hide the emote from all normal areas, however it can still be invoked through PlayAnimation, use this for emotes that are only needed in code
        /// </summary>
        public bool visible = true;

        /// <summary>
        /// If true, will sync animation between all people emoting
        /// </summary>
        public bool syncAnim = false;

        /// <summary>
        /// If true, will sync audio between all people emoting
        /// </summary>
        public bool syncAudio = false;

        /// <summary>
        /// Spot in primaryAnimationClips array where a BoneMapper will play when there is no other instance of said emote playing -1 is random, -2 is sequential, anything else is what you make it to be
        /// </summary>
        public int startPref = -1;

        /// <summary>
        /// Spot in primaryAnimationClips array where a BoneMapper will play when there is at least one other instance of said emote playing, -1 is random, -2 is sequential, anything else is what you make it to be
        /// </summary>
        public int joinPref = -1;

        /// <summary>
        /// Array of join spots which will appear when the animation is playing
        /// </summary>
        public JoinSpot[] joinSpots = null;

        /// <summary>
        /// Use this if you want a custom internal name for the emote, if not specified, will auto create it based off of the first item in primaryAnimationClips
        /// </summary>
        public string internalName = "";

        /// <summary>
        /// Changes the user facing name, you should use this, can have duplicates
        /// </summary>
        public string displayName = "";

        /// <summary>
        /// If declared, will fire when an emote plays audio with sync enabled
        /// </summary>
        public Action<BoneMapper> customPostEventCodeSync = null;

        /// <summary>
        /// If declared, will fire when an emote plays audio with sync disabled
        /// </summary>
        public Action<BoneMapper> customPostEventCodeNoSync = null;

        /// <summary>
        /// Determines the lock type of your emote, none, headBobbing, lockHead, or rootMotion
        /// </summary>
        public LockType lockType = LockType.none;

        /// <summary>
        /// Lets you mark if your normal set of audio will get claimed by DMCA
        /// </summary>
        public bool willGetClaimedByDMCA = true;

        /// <summary>
        /// Determines the volume of the emote in terms of alerting enemies, 0 is nothing, 1 is max
        /// </summary>
        public float audioLevel = .5f;

        /// <summary>
        /// If true, will default animation to third person, although there are user settings to override this in either direction
        /// </summary>
        public bool thirdPerson = false;

        /// <summary>
        /// Will ignore any bones declared on this list and all of their child bones (recursive) from the animation that plays
        /// </summary>
        public HumanBodyBones[] rootBonesToIgnore = null;

        /// <summary>
        /// Will ignore any bones declared on this list from the animation that plays
        /// </summary>
        public HumanBodyBones[] soloBonesToIgnore = null;

        /// <summary>
        /// Will use local positions instead of global positions, used primarily with root/soloBonesToIgnore
        /// </summary>
        public bool useLocalTransforms = false;

        /// <summary>
        /// <see cref="BepInPlugin"/> of the mod that created this <see cref="AnimationClipParams"/>.
        /// </summary>
        public BepInPlugin OwnerPlugin { get; } = Assembly.GetCallingAssembly().GetBepInPlugin();

        /// <summary>
        /// Declare if your emote will not directly call animation. AnimationClips passed into earlier parameters will still be used for preview purposes, leave those null if not needed.
        /// </summary>
        public bool nonAnimatingEmote = false;




        //keeping this for if we ever port to a game that needs them so I don't forget
        //public bool useSafePositionReset = false; // Unused in Lethal Company
        //public bool stopWhenAttack = false; // Unused in Lethal Company
        //public bool dimWhenClose = false; // Unused in Lethal Company
    }
}
