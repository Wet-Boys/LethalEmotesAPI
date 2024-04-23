using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Editor.Utils;
using Editor.Utils.Uxml;
using LEAPI.UI.Toolkit;
using LethalEmotesApi.Ui;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(EmoteUiPanel))]
    public class EmoteUiPanelEditor : LeUiCustomEditorBase<EmoteUiPanel>
    {
        public VisualTreeAsset uxmlVisualTree;

        [UxmlBindValue("EmoteGenSettings.EmoteModsToGenField")]
        private int _emoteModsToGen = 10;
        
        [UxmlBindValue("EmoteGenSettings.EmotesPerEmoteModField")]
        private int _emotesPerEmoteMod = 100;

        private Label _dataLoadedLabel;
        
        protected override VisualElement CreateGUI()
        {
            var root = new VisualElement();

            if (!uxmlVisualTree)
                return root;

            uxmlVisualTree.CloneTree(root);

            return root;
        }

        private void UpdateStatusLabel()
        {
            _dataLoadedLabel.text = IsDataLoaded() ? "Loaded" : "Not Loaded";
        }

        [UxmlBindButton("LoadFakeDataButton")]
        private void LoadData()
        {
            var stubbedState = new EmoteUiDataLoader.StubbedState();

            var emoteDb = (EmoteUiDataLoader.StubbedEmoteDb)stubbedState.EmoteDb;
            emoteDb.GenerateData(_emoteModsToGen, _emotesPerEmoteMod);
            
            EmoteUiManager.RegisterStateController(stubbedState);

            UpdateStatusLabel();
            
            Repaint();
        }
        
        [UxmlBindButton("ClearFakeDataButton")]
        private void ClearData()
        {
            var type = typeof(EmoteUiManager);
            var field = type.GetField("_stateController", BindingFlags.NonPublic | BindingFlags.Static);
            
            field?.SetValue(null, null);
            
            UpdateStatusLabel();
            
            Repaint();
        }

        private bool IsDataLoaded()
        {
            var type = typeof(EmoteUiManager);
            var field = type.GetField("_stateController", BindingFlags.NonPublic | BindingFlags.Static);

            var value = field?.GetValue(null);

            return value != null;
        }

        [UxmlOnBindElement("StatusGroup.FakeDataStatusLabel")]
        private void OnLabelBind(VisualElement label)
        {
            _dataLoadedLabel = (Label)label;
        }
    }
}