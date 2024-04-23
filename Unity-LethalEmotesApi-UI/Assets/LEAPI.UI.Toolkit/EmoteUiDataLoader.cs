using System;
using System.Collections.Generic;
using System.Linq;
using LethalEmotesApi.Ui;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace LEAPI.UI.Toolkit
{
    public class EmoteUiDataLoader : MonoBehaviour
    {
        public EmoteUiPanel emoteUiPanel;
        public int emoteModsToCreate;
        public int emotesPerEmoteMod;

        private void Start()
        {
            var customize = emoteUiPanel.customizePanel;
            if (customize is null)
                return;
            
            var stubbedState = new StubbedState();

            var emoteDb = (StubbedEmoteDb)stubbedState.EmoteDb;
            emoteDb.GenerateData(emoteModsToCreate, emotesPerEmoteMod);
            
            EmoteUiManager.RegisterStateController(stubbedState);
            
            customize.gameObject.SetActive(false);
            customize.gameObject.SetActive(true);
        }

        public class StubbedState : IEmoteUiStateController
        {
            private readonly List<string> _randomBlacklist = new();
            private readonly List<string> _emoteBlacklist = new();

            private EmoteWheelSetData _data = null;
            
            public void PlayEmote(string emoteKey) { }

            public void LockMouseInput() { }

            public void UnlockMouseInput() { }

            public void LockPlayerInput() { }

            public void UnlockPlayerInput() { }

            public bool CanOpenEmoteUi() => true;

            public void PlayAnimationOn(Animator animator, string emoteKey) { }

            public string GetEmoteName(string emoteKey) => emoteKey;

            public void AddToRandomPoolBlacklist(string emoteKey)
            {
                if (_randomBlacklist.Contains(emoteKey))
                    return;
                
                _randomBlacklist.Add(emoteKey);
            }

            public void RemoveFromRandomPoolBlacklist(string emoteKey) => _randomBlacklist.Remove(emoteKey);
            public void AddToEmoteBlacklist(string emoteKey)
            {
                if (_emoteBlacklist.Contains(emoteKey))
                    return;
                
                _emoteBlacklist.Add(emoteKey);
            }

            public void RemoveFromEmoteBlacklist(string emoteKey) => _emoteBlacklist.Remove(emoteKey);
            
            public void RefreshBothLists() { }
            
            public InputActionReference GetEmoteKeybind(string emoteKey) => null;
            
            public void EnableKeybinds() { }
    
            public void DisableKeybinds() { }

            public string[] GetEmoteKeysForBindPath(string bindPath) => Array.Empty<string>();
            public T LoadAsset<T>(string assetName) where T : Object => null;
            public void EnqueueWorkOnUnityThread(Action action)
            {
                Debug.Log("No Thread Scheduler in UnityEditor!");
            }

            public void LoadKeybinds() { }

            public void RefreshTME() { }

            public EmoteWheelSetData LoadEmoteWheelSetData()
            {
                _data ??= new EmoteWheelSetData
                {
                    EmoteWheels = new[]
                    {
                        EmoteWheelData.CreateDefault(), EmoteWheelData.CreateDefault(1), EmoteWheelData.CreateDefault(2),
                        EmoteWheelData.CreateDefault(3)
                    }
                };

                return _data;
            }

            public EmoteWheelSetDisplayData LoadEmoteWheelSetDisplayData()
            {
                return new EmoteWheelSetDisplayData();
            }
            
            public void SaveKeybinds() { }

            public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
            {
                _data = dataToSave;
            }

            public void SaveEmoteWheelSetDisplayData(EmoteWheelSetDisplayData dataToSave) { }


            public IEmoteDb EmoteDb { get; } = new StubbedEmoteDb();

            public IEmoteHistoryManager EmoteHistoryManager { get; } = new StubbedEmoteHistoryManager();

            public IReadOnlyCollection<string> RandomPoolBlacklist => _randomBlacklist.ToArray();
            public IReadOnlyCollection<string> EmotePoolBlacklist => _emoteBlacklist.ToArray();
            public float EmoteVolume { get; set; }
            public bool HideJoinSpots { get; set; }
            public int RootMotionType { get; set; }
            public bool EmotesAlertEnemies { get; set; }
            public int DmcaFree { get; set; }
            public int ThirdPerson { get; set; }
            public bool UseGlobalSettings { get; set; }
            public bool DontShowDmcaPrompt { get; set; }
        }
        
        public class StubbedEmoteDb : IEmoteDb
        {
            public string GetEmoteName(string emoteKey) => emoteKey.Split('.').Last();

            public void AssociateEmoteKeyWithMod(string emoteKey, string modName) { }

            private Dictionary<string, string> _emoteMods = new();
            
            public string GetModName(string emoteKey) => _emoteMods[emoteKey];

            public string GetModNameFromModGuid(string modGuid) => modGuid;

            public bool GetEmoteVisibility(string emoteKey) => true;

            private string[] _emoteKeys = Array.Empty<string>();
            
            public IReadOnlyCollection<string> EmoteKeys
            {
                get
                {
                    if (_emoteKeys is null)
                    {
                        _emoteKeys = new string[100];
                        for (int i = 0; i < _emoteKeys.Length; i++)
                        {
                            _emoteKeys[i] = $"Emote {i + 1}";
                        }
                    }

                    return _emoteKeys;
                }
            }

            public IReadOnlyCollection<string> EmoteModNames => _emoteMods.Select(kvp => kvp.Value)
                .Distinct()
                .ToArray();

            public IReadOnlyCollection<string> EmoteModGuids => EmoteModNames;

            public void GenerateData(int emoteModsToGen, int emotesPerMod)
            {
                var keys = new List<string>();
                
                for (int i = 0; i < emoteModsToGen; i++)
                {
                    var modName = $"EmoteMod {i + 1}";

                    for (int j = 0; j < emotesPerMod; j++)
                    {
                        var key = $"{modName}.Emote {j + 1}";
                        keys.Add(key);
                        _emoteMods[key] = modName;
                    }
                }

                _emoteKeys = keys.ToArray();
            }
        }

        public class StubbedEmoteHistoryManager : IEmoteHistoryManager
        {
            
            
            public void GenerateData()
            {
                
            }
            
            public void PlayerPerformedEmote(float dist, string emoteKey, string playerName)
            {
                
            }

            public RecentEmote GetRecentEmote(string emoteKey)
            {
                return new RecentEmote(emoteKey, "Nobody");
            }

            public RecentEmote[] GetCurrentlyPlayingEmotes()
            {
                return Array.Empty<RecentEmote>();
            }

            public RecentEmote[] GetHistory()
            {
                return Array.Empty<RecentEmote>();
            }
        }
    }
}
