﻿using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CustomAnimationClip : MonoBehaviour
{
    public AnimationClip[] clip, secondaryClip; //DONT SUPPORT MULTI CLIP ANIMATIONS TO SYNC     //but why not? how hard could it be, I'm sure I left that note for a reason....  //it was for a reason, but it works now
    public bool looping;
    public string wwiseEvent;
    public bool syncronizeAudio;
    public List<HumanBodyBones> soloIgnoredBones;
    public List<HumanBodyBones> rootIgnoredBones;
    public bool dimAudioWhenClose;
    public bool stopOnAttack;
    public bool stopOnMove;
    public bool visibility;
    public int startPref, joinPref;
    public JoinSpot[] joinSpots;
    public bool useSafePositionReset;
    public string customName;
    public Action<BoneMapper> customPostEventCodeSync;
    public Action<BoneMapper> customPostEventCodeNoSync;


    public bool syncronizeAnimation;
    public int syncPos;
    public static List<float> syncTimer = new List<float>();
    public static List<int> syncPlayerCount = new List<int>();
    public static List<List<bool>> uniqueAnimations = new List<List<bool>>();
    public bool vulnerableEmote = false;
    public AnimationClipParams.LockType lockType = AnimationClipParams.LockType.none;
    public bool willGetClaimed = false;
    public float audioLevel = .5f;
    public bool thirdPerson = false;

    internal CustomAnimationClip(AnimationClip[] _clip, bool _loop, AudioClip[] primaryAudioClips = null, AudioClip[] secondaryAudioClips = null, HumanBodyBones[] rootBonesToIgnore = null, HumanBodyBones[] soloBonesToIgnore = null, AnimationClip[] _secondaryClip = null, bool dimWhenClose = false, bool stopWhenMove = false, bool stopWhenAttack = false, bool visible = true, bool syncAnim = false, bool syncAudio = false, int startPreference = -1, int joinPreference = -1, JoinSpot[] _joinSpots = null, bool safePositionReset = false, string customName = "", Action<BoneMapper> _customPostEventCodeSync = null, Action<BoneMapper> _customPostEventCodeNoSync = null, AnimationClipParams.LockType lockType = AnimationClipParams.LockType.none, AudioClip[] primaryDMCAFreeAudioClips = null, AudioClip[] secondaryDMCAFreeAudioClips = null, bool willGetClaimed = false, float audioLevel = .5f, bool thirdPerson = false)
    {
        if (rootBonesToIgnore == null)
            rootBonesToIgnore = new HumanBodyBones[0];
        if (soloBonesToIgnore == null)
            soloBonesToIgnore = new HumanBodyBones[0];
        clip = _clip;
        secondaryClip = _secondaryClip;
        looping = _loop;
        dimAudioWhenClose = dimWhenClose;
        stopOnAttack = stopWhenAttack;
        stopOnMove = stopWhenMove;
        visibility = visible;
        joinPref = joinPreference;
        startPref = startPreference;
        customPostEventCodeSync = _customPostEventCodeSync;
        customPostEventCodeNoSync = _customPostEventCodeNoSync;
        if (primaryAudioClips == null)
        {
            BoneMapper.primaryAudioClips.Add(new AudioClip[] { null });
        }
        else
        {
            BoneMapper.primaryAudioClips.Add(primaryAudioClips);
        }
        if (secondaryAudioClips == null)
        {
            BoneMapper.secondaryAudioClips.Add(new AudioClip[] { null });
        }
        else
        {
            BoneMapper.secondaryAudioClips.Add(secondaryAudioClips);
        }

        if (primaryDMCAFreeAudioClips == null)
        {
            BoneMapper.primaryDMCAFreeAudioClips.Add(new AudioClip[] { null });
        }
        else
        {
            BoneMapper.primaryDMCAFreeAudioClips.Add(primaryDMCAFreeAudioClips);
        }
        if (secondaryDMCAFreeAudioClips == null)
        {
            BoneMapper.secondaryDMCAFreeAudioClips.Add(new AudioClip[] { null });
        }
        else
        {
            BoneMapper.secondaryDMCAFreeAudioClips.Add(secondaryDMCAFreeAudioClips);
        }

        if (soloBonesToIgnore.Length != 0)
        {
            soloIgnoredBones = new List<HumanBodyBones>(soloBonesToIgnore);
        }
        else
        {
            soloIgnoredBones = new List<HumanBodyBones>();
        }

        if (rootBonesToIgnore.Length != 0)
        {
            rootIgnoredBones = new List<HumanBodyBones>(rootBonesToIgnore);
        }
        else
        {
            rootIgnoredBones = new List<HumanBodyBones>();
        }
        syncronizeAnimation = syncAnim;
        syncronizeAudio = syncAudio;
        syncPos = syncTimer.Count;
        syncTimer.Add(0);
        syncPlayerCount.Add(0);
        List<bool> bools = new List<bool>();
        for (int i = 0; i < _clip.Length; i++)
        {
            bools.Add(false);
        }
        uniqueAnimations.Add(bools);

        if (_joinSpots == null)
            _joinSpots = new JoinSpot[0];
        joinSpots = _joinSpots;
        this.useSafePositionReset = safePositionReset;
        this.customName = customName;
        if (customName != "")
        {
            BoneMapper.customNamePairs.Add(customName, _clip[0].name);
        }
        else
        {
            this.customName = clip[0].name;
        }
        BoneMapper.listOfCurrentEmoteAudio.Add(new List<AudioSource>());
        this.lockType = lockType;
        this.willGetClaimed = willGetClaimed;
        this.audioLevel = audioLevel;
        this.thirdPerson = thirdPerson;
    }
}