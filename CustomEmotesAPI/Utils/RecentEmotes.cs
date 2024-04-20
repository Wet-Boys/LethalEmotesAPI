using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Utils
{
    public class RecentEmote
    {
        private static Dictionary<string, RecentEmote> emoteKeysToNearbyEmote = new Dictionary<string, RecentEmote>();
        public float distance; //idk if we actually need distance...
        public List<string> playerNames;
        public string emoteKey;
        private RecentEmote(float distance, string playerName, string emoteKey)
        {
            this.distance = distance;
            this.playerNames = new List<string>([playerName]);
            this.emoteKey = emoteKey;
        }
        private bool AddPlayer(string playerName)
        {
            if (!playerNames.Contains(playerName))
            {
                playerNames.Add(playerName);
                return true;
            }
            return false;
        }
        internal static void PlayerPerformedEmote(float dist, string emoteKey, string playerName) //gets fired whenever a BoneMapper calls an emote
        {
            if (emoteKeysToNearbyEmote.ContainsKey(emoteKey))
            {
                emoteKeysToNearbyEmote[emoteKey].AddPlayer(playerName);
                RecentEmote recent = emoteKeysToNearbyEmote[emoteKey];
                emoteKeysToNearbyEmote.Remove(emoteKey);
                emoteKeysToNearbyEmote.Add(emoteKey, recent); //increase player count and re-add item to back of list (just read it backwards)
            }
            else
            {
                emoteKeysToNearbyEmote.Add(emoteKey, new RecentEmote(dist, playerName, emoteKey));
            }
            while (emoteKeysToNearbyEmote.Count > 50)
            {
                emoteKeysToNearbyEmote.Remove(emoteKeysToNearbyEmote.First().Key);
            }
            playerEmoted(emoteKeysToNearbyEmote.Last().Value);
        }
        public static RecentEmote GetRecentEmote(string emoteKey)//idk if this is needed tbh
        {
            return emoteKeysToNearbyEmote[emoteKey];
        }
        public static RecentEmote[] GetRecentEmotes()//so my thought is this can be used for the tab you are making in the customize menu.
                                                     //But also, for the emote wheel section, call this once when opening the wheel and then use event below to add new items to the bottom of the list as players emote with the wheel already open.
                                                     //You could almost just sort the emote wheel section oldest to newest to fix that but I don't think we have space for all that...
        {
            return emoteKeysToNearbyEmote.Values.ToArray();
        }
        public delegate void PlayerPerformedEmoteEvent(RecentEmote recentEmote);
        public static event PlayerPerformedEmoteEvent playerPerformedEmoteEvent;
        internal static void playerEmoted(RecentEmote recentEmote)
        {
            if (playerPerformedEmoteEvent != null)
                playerPerformedEmoteEvent(recentEmote);
        }
    }
}
