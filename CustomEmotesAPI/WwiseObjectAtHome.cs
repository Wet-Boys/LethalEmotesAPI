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
        internal void Setup(AudioSource source)
        {
            audioSource = source;
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
        }
        public void Play(int syncPos, int currEvent, bool looping)
        {
            this.syncPos = syncPos;
            this.currEvent = currEvent;
            try
            {
                audioSource.clip = BoneMapper.primaryAudioClips[syncPos][currEvent];
                audioSource.Play();
                if (BoneMapper.secondaryAudioClips[syncPos][currEvent] != null)
                {
                    needToContinueOnFinish = true;
                }
                else
                {
                    audioSource.loop = looping;
                    needToContinueOnFinish = false;
                }
            }
            catch (Exception)
            {
                DebugClass.Log($"{syncPos}  {BoneMapper.primaryAudioClips[syncPos]}  {BoneMapper.primaryAudioClips[syncPos].Length}           {BoneMapper.secondaryAudioClips[syncPos]}   {BoneMapper.secondaryAudioClips[syncPos].Length}");
            }
        }
        public void Stop()
        {
            audioSource.Stop();
        }
        //public void Pause()
        //{

        //}
    }
}
