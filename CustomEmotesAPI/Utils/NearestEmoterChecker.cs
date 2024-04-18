using EmotesAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
            Debug.Log($"checking ============ {self.currentClipName}");
            if (self.currentClipName == "none" && CustomEmotesAPI.currentEmoteText is not null)
            {
                Debug.Log($"checking ============");

                float closestDistance = 30f;
                BoneMapper nearestMapper = null;
                foreach (var mapper in BoneMapper.allMappers)
                {
                    try
                    {
                        if (mapper != self)
                        {
                            float dist = Vector3.Distance(self.transform.position, mapper.transform.position);
                            if (mapper.currentClip.allowJoining && dist < closestDistance)
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
                    string animationName;
                    if (nearestMapper.currentClip.usesNewImportSystem)
                    {
                        animationName = nearestMapper.currentClip.joinEmote;
                    }
                    else
                    {
                        animationName = nearestMapper.currentClip.clip[0].name;
                    }
                    //CustomEmotesAPI.currentEmoteText.text = $"{} is playing {nearestMapper.currentClip.displayName} press {} to join";
                    string name = nearestMapper.isEnemy ? nearestMapper.enemyController.enemyType.enemyName : nearestMapper.playerController.playerUsername;
                    CustomEmotesAPI.currentEmoteText.text = $"Press V to join {name}";
                }
                else
                {
                    CustomEmotesAPI.currentEmoteText.text = "";
                }
            }
            StartCoroutine(CheckOnInterval(1));
        }
    }
}
