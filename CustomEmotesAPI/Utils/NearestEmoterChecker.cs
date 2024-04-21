using EmotesAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalEmotesAPI.Utils
{
    internal class NearestEmoterChecker : MonoBehaviour
    {
        public BoneMapper self;
        void Start()
        {
            StartCoroutine(CheckOnInterval(1));
        }
        IEnumerator CheckOnInterval(float interval)
        {
            yield return new WaitForSeconds(interval);
            if ((Settings.joinEmoteTutorial.Value || Settings.NearestEmoteText.Value) && CustomEmotesAPI.currentEmoteText is not null)
            {
                if (self.currentClipName == "none")
                {
                    float closestDistance = 30f;
                    BoneMapper nearestMapper = null;
                    foreach (var mapper in BoneMapper.allMappers)
                    {
                        try
                        {
                            if (mapper != self)
                            {
                                float dist = Vector3.Distance(self.transform.position, mapper.transform.position);
                                if (mapper.currentClip is not null && mapper.currentClip.allowJoining && dist < closestDistance)
                                {
                                    nearestMapper = mapper;
                                    closestDistance = dist;
                                }
                            }
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                    if (nearestMapper is not null)
                    {
                        string currentJoinButton = InputControlPath.ToHumanReadableString(
    EmotesInputSettings.Instance.JoinEmote.bindings[0].effectivePath,
    InputControlPath.HumanReadableStringOptions.OmitDevice);
                        CustomEmotesAPI.currentEmoteText.text = $"{currentJoinButton} to join {nearestMapper.currentClip.displayName}";
                    }
                    else
                    {
                        CustomEmotesAPI.currentEmoteText.text = "";
                    }
                }
            }
            StartCoroutine(CheckOnInterval(interval));
        }
    }
}
