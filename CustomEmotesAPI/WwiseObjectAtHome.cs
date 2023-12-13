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
                audioSource.clip = BoneMapper.secondaryAudioClips[syncPos][currEvent];
                audioSource.Play();
                needToContinueOnFinish = false;
                audioSource.loop = true;
            }
            if (audioSource.isPlaying)
            {
                audioTimer += Time.deltaTime;
                if (audioTimer > .75f)
                {
                    audioSource.volume = Settings.EmotesVolume.Value / 100f;
                    audioTimer -= .75f;
                    if (Settings.EmotesAlertEnemies.Value)
                    {
                        RoundManager.Instance.PlayAudibleNoise(mapper.mapperBody.transform.position, 30, .75f, 0, mapper.mapperBody.isInHangarShipRoom && mapper.mapperBody.playersManager.hangarDoorsClosed, 5);
                    }
                }
            }
        }
        public void Play(int syncPos, int currEvent, bool looping, bool sync)
        {
            //emotes with multiple song choices are causing errors
            this.syncPos = syncPos;
            this.currEvent = currEvent;
            if (BoneMapper.listOfCurrentEmoteAudio[syncPos].Count != 0 && sync)
            {
                this.currEvent = BoneMapper.listOfCurrentEmoteAudio[syncPos][0].gameObject.transform.parent.GetComponent<BoneMapper>().currEvent;
            }

            if (BoneMapper.secondaryAudioClips[syncPos].Length > currEvent && BoneMapper.secondaryAudioClips[syncPos][currEvent] != null)
            {
                if (CustomAnimationClip.syncTimer[syncPos] > BoneMapper.primaryAudioClips[syncPos][currEvent].length)
                {
                    SetAndPlayAudio(BoneMapper.secondaryAudioClips[syncPos][currEvent]);
                    SampleCheck();
                    needToContinueOnFinish = false;
                    audioSource.loop = true;
                }
                else
                {
                    SetAndPlayAudio(BoneMapper.primaryAudioClips[syncPos][currEvent]);
                    SampleCheck();
                    needToContinueOnFinish = true;
                    audioSource.loop = false;
                }
            }
            else if (looping)
            {
                SetAndPlayAudio(BoneMapper.primaryAudioClips[syncPos][currEvent]);
                SampleCheck();
                needToContinueOnFinish = false;
                audioSource.loop = true;
            }
            else
            {
                SetAndPlayAudio(BoneMapper.primaryAudioClips[syncPos][currEvent]);
                SampleCheck();
                needToContinueOnFinish = false;
                audioSource.loop = false;
            }
        }
        public void Stop()
        {
            audioSource.Stop();
            needToContinueOnFinish = false;
        }
        public void SetAndPlayAudio(AudioClip a)
        {
            audioSource.clip = a;
            audioSource.Play();
        }
        public void SampleCheck()
        {
            if (BoneMapper.listOfCurrentEmoteAudio[syncPos].Count != 0)
            {
                audioSource.timeSamples = BoneMapper.listOfCurrentEmoteAudio[syncPos][0].timeSamples;
                var theBusStopProblem = gameObject.AddComponent<BusStop>();
                theBusStopProblem.desiredSampler = BoneMapper.listOfCurrentEmoteAudio[syncPos][0];
                theBusStopProblem.receiverSampler = audioSource;
            }
            else
            {
                audioSource.timeSamples = 0;
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
                DestroyImmediate(this);
            }
            if (desiredSampler.timeSamples != receiverSampler.timeSamples)
            {
                receiverSampler.timeSamples = desiredSampler.timeSamples;
                success = 0;
            }
            else
            {
                success++;
                if (success == 2)
                {
                    DestroyImmediate(this);
                }
            }
        }
    }
}
