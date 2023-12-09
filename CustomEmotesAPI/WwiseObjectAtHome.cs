using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI
{
    public class AudioManager : MonoBehaviour
    {
        //I miss wwise :(
        public AudioClip primaryAudioClip;
        public AudioClip secondaryAudioClip;
        public bool primaryIsCurrent = true;
        public AudioManager(AudioClip primaryClip, AudioClip secondaryClip)
        {
            primaryAudioClip = primaryClip;
            secondaryAudioClip = secondaryClip;
        }

        private void Start()
        {

        }
        private void Update()
        {

        }
        public void Play()
        {
            if (primaryIsCurrent)
            {

            }
        }
        public void Stop()
        {

        }
        public void Pause()
        {
            
        }
    }
}
