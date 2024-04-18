using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Utils
{
    public struct NearbyEmote(float distance, string playerName, string emoteKey)
    {
        float distance = distance;
        string playerName = playerName;
        string emoteKey = emoteKey;
    }
    public class NearbyEmoteChecker
    {
        public static NearbyEmote[] GetNearbyEmotes(float maxDistance)
        {
            List<NearbyEmote> nearbyEmotes = new List<NearbyEmote>();
            foreach (var item in BoneMapper.allMappers)
            {
                if (!item.local && item.currentClip is not null)
                {
                    float dist = Vector3.Distance(item.transform.position, CustomEmotesAPI.localMapper.transform.position);
                    if (dist <= maxDistance)
                    {
                        nearbyEmotes.Add(new NearbyEmote(dist, item.playerController.playerUsername, item.currentClip.customInternalName));
                    }
                }
            }
            return nearbyEmotes.ToArray();
        }
    }
}
