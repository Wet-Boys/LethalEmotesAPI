using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI
{
    public class AudioManager : MonoBehaviour
    {
        //I miss wwise :(
        public AudioSource audioSource;
        public bool needToContinueOnFinish = false;
        int syncPos, currEvent;
        public BoneMapper mapper;
        float audioTimer = 0;
        bool currentClipDMCA;
        internal void Setup(AudioSource source, BoneMapper mapper)
        {
            audioSource = source;
            this.mapper = mapper;
        }
        private void Start()
        {

        }
        private void Update()
        {
            if (!audioSource.isPlaying && needToContinueOnFinish)
            {
                if (currentClipDMCA && Settings.DMCAFree.Value == DMCAType.Mute)
                {
                    needToContinueOnFinish = false;
                    return;
                }
                audioSource.time = 0;
                if (CheckDMCA(currentClipDMCA))
                {
                    audioSource.clip = BoneMapper.secondaryDMCAFreeAudioClips[syncPos][currEvent];
                }
                else
                {
                    audioSource.clip = BoneMapper.secondaryAudioClips[syncPos][currEvent];
                }
                audioSource.Play();
                needToContinueOnFinish = false;
                audioSource.loop = true;
            }
            if (audioSource.isPlaying)
            {
                    audioSource.volume = Settings.EmotesVolume.Value / 100f;
                    
                    if (Settings.EmotesAlertEnemies.Value && mapper.playerController is not null && mapper.playerController.IsOwner)
                    {
                        if (audioTimer <= 0f)
                        {
                            audioTimer = 1f;
                            RoundManager.Instance.PlayAudibleNoise(mapper.mapperBody.transform.position, 30f, mapper.currentAudioLevel, 0, mapper.playerController.isInHangarShipRoom && mapper.playerController.playersManager.hangarDoorsClosed, 5);
                        }
                        else
                        {
                            audioTimer -= Time.deltaTime;
                        }
                    }
            }

        }
        public void Play(int syncPos, int currEvent, bool looping, bool sync, bool willGetClaimed)
        {
            currentClipDMCA = willGetClaimed;
            if (currentClipDMCA && Settings.DMCAFree.Value == DMCAType.Mute)
            {
                return;
            }
            audioSource.time = 0;
            this.syncPos = syncPos;
            this.currEvent = currEvent;
            if (BoneMapper.listOfCurrentEmoteAudio[syncPos].Count != 0 && sync)
            {
                this.currEvent = BoneMapper.listOfCurrentEmoteAudio[syncPos][0].gameObject.transform.parent.GetComponentInChildren<BoneMapper>().currEvent;
                currEvent = this.currEvent;
            }
            if (BoneMapper.secondaryAudioClips[syncPos].Length > currEvent && BoneMapper.secondaryAudioClips[syncPos][currEvent] != null)
            {
                if (CustomAnimationClip.syncTimer[syncPos] > BoneMapper.primaryAudioClips[syncPos][currEvent].length)
                {
                    if (CheckDMCA(willGetClaimed))
                    {
                        SetAndPlayAudio(BoneMapper.secondaryDMCAFreeAudioClips[syncPos][currEvent]);
                    }
                    else
                    {
                        SetAndPlayAudio(BoneMapper.secondaryAudioClips[syncPos][currEvent]);
                    }
                    SampleCheck();
                    needToContinueOnFinish = false;
                    audioSource.loop = true;
                }
                else
                {
                    if (CheckDMCA(willGetClaimed))
                    {
                        SetAndPlayAudio(BoneMapper.primaryDMCAFreeAudioClips[syncPos][currEvent]);
                    }
                    else
                    {
                        SetAndPlayAudio(BoneMapper.primaryAudioClips[syncPos][currEvent]);
                    }
                    SampleCheck();
                    needToContinueOnFinish = true;
                    audioSource.loop = false;
                }
            }
            else if (looping)
            {
                if (CheckDMCA(willGetClaimed))
                {
                    SetAndPlayAudio(BoneMapper.primaryDMCAFreeAudioClips[syncPos][currEvent]);
                }
                else
                {
                    SetAndPlayAudio(BoneMapper.primaryAudioClips[syncPos][currEvent]);
                }
                SampleCheck();
                needToContinueOnFinish = false;
                audioSource.loop = true;
            }
            else
            {
                if (CheckDMCA(willGetClaimed))
                {
                    SetAndPlayAudio(BoneMapper.primaryDMCAFreeAudioClips[syncPos][currEvent]);
                }
                else
                {
                    SetAndPlayAudio(BoneMapper.primaryAudioClips[syncPos][currEvent]);
                }
                SampleCheck();
                needToContinueOnFinish = false;
                audioSource.loop = false;
            }
        }
        public void Stop()
        {
            needToContinueOnFinish = false;
            audioSource.Stop();
        }
        public void SetAndPlayAudio(AudioClip a)
        {
            if (a)
            {
                audioSource.clip = a;
                audioSource.volume = 0;
                audioSource.Play();
            }
            else
            {
                needToContinueOnFinish = false;
            }
        }
        public void SampleCheck()
        {
            if (BoneMapper.listOfCurrentEmoteAudio[syncPos].Count != 0)
            {
                audioSource.time = BoneMapper.listOfCurrentEmoteAudio[syncPos][0].time;
                var theBusStopProblem = gameObject.AddComponent<BusStop>();
                theBusStopProblem.desiredSampler = BoneMapper.listOfCurrentEmoteAudio[syncPos][0];
                theBusStopProblem.receiverSampler = audioSource;
            }
            else
            {
                audioSource.volume = Settings.EmotesVolume.Value / 100f;
                audioSource.time = 0;
            }
        }
        public bool CheckDMCA(bool willGetClaimed)
        {
            switch (Settings.DMCAFree.Value)
            {
                case DMCAType.Normal:
                    //all songs are normal
                    return false;
                case DMCAType.Friendly:
                //based on import settings
                case DMCAType.Mute:
                    //need to just mute the song
                    return willGetClaimed;
                case DMCAType.AllOff:
                    //all songs are dmca free or quiet if no dmca track is given
                    return true;
                default:
                    return true;
            }
        }
    }

    public class BusStop : MonoBehaviour
    {
        public AudioSource desiredSampler;
        public AudioSource receiverSampler;
        int success = 0;
        private void Update()
        {
            if (!desiredSampler)
            {
                receiverSampler.volume = Settings.EmotesVolume.Value / 100f;
                DestroyImmediate(this);
            }
            if (desiredSampler.time != receiverSampler.time)
            {
                receiverSampler.volume = 0;
                receiverSampler.time = desiredSampler.time;
                success = 0;
            }
            else
            {
                success++;
                if (success == 3)
                {
                    receiverSampler.volume = Settings.EmotesVolume.Value / 100f;
                    DestroyImmediate(this);
                }
            }
        }
    }
}
