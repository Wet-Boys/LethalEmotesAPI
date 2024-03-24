using System.Collections.Generic;
using System.Reflection;
using Editor.Utils;
using JetBrains.Annotations;
using LethalEmotesApi.Ui;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(EmoteUiPanel))]
    public class EmoteUiPanelEditor : LeUiCustomEditorBase<EmoteUiPanel>
    {
        
        
        protected override VisualElement CreateGUI()
        {
            var root = new VisualElement();

            var loadDataButton = new Button(LoadData)
            {
                text = "Load Fake Data"
            };

            var clearDataButton = new Button(ClearData)
            {
                text = "Clear Fake Data"
            };

            var statusText = IsDataLoaded() ? "Loaded" : "Not Loaded";

            var fakeDataStatus = new Label($"Fake Data Status: {statusText}");
            
            root.Add(fakeDataStatus);
            root.Add(loadDataButton);
            root.Add(clearDataButton);

            return root;
        }

        private void LoadData()
        {
            EmoteUiManager.RegisterStateController(new StubbedState());
            
            Repaint();
        }
        
        private void ClearData()
        {
            var type = typeof(EmoteUiManager);
            var field = type.GetField("_stateController", BindingFlags.NonPublic | BindingFlags.Static);
            
            field?.SetValue(null, null);
            
            Repaint();
        }

        private bool IsDataLoaded()
        {
            var type = typeof(EmoteUiManager);
            var field = type.GetField("_stateController", BindingFlags.NonPublic | BindingFlags.Static);

            var value = field?.GetValue(null);

            return value != null;
        }
        
        
        private class StubbedState : IEmoteUiStateController
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

            public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
            {
                _data = dataToSave;
            }

            public IEmoteDb EmoteDb { get; }

            public IReadOnlyCollection<string> EmoteKeys { get; } = new[] { "none" };
            public IReadOnlyCollection<string> RandomPoolBlacklist => _randomBlacklist.ToArray();
            public IReadOnlyCollection<string> EmotePoolBlacklist => _emoteBlacklist.ToArray();
            public float EmoteVolume { get; set; }
            public bool HideJoinSpots { get; set; }
            public int RootMotionType { get; set; }
            public bool EmotesAlertEnemies { get; set; }
            public int DmcaFree { get; set; }
            public int ThirdPerson { get; set; }
            public bool UseGlobalSettings { get; set; }
        }
    }
}