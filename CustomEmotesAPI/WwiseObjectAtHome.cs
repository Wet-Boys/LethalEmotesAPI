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
        public bool needToContinueOnFinish = true;
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
                audioSource.PlayOneShot(BoneMapper.secondaryAudioClips[syncPos][currEvent]);
                needToContinueOnFinish = false;
                audioSource.loop = true;
            }
        }
        public void Play(int syncPos, int currEvent, bool looping)
        {
            this.syncPos = syncPos;
            this.currEvent = currEvent;
            audioSource.PlayOneShot(BoneMapper.primaryAudioClips[syncPos][currEvent]);
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
        public void Stop()
        {
            audioSource.Stop();
        }
        //public void Pause()
        //{

        //}
    }
}
