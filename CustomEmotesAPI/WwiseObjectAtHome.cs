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
        public AudioSource audioSource;
        public bool primaryIsCurrent = true;
        public AudioManager(AudioSource source)
        {
            audioSource = source;
        }

        private void Start()
        {

        }
        private void Update()
        {

        }
        public void Play(int syncPos, int currEvent, bool looping)
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
