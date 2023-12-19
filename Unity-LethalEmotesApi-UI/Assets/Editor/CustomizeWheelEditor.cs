using System.Collections.Generic;
using Editor.Utils;
using LethalEmotesApi.Ui.Customize.Wheel;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(CustomizeWheel))]
    public class CustomizeWheelEditor : LeUiCustomEditorBase<CustomizeWheel>
    {
        protected override VisualElement CreateGUI()
        {
            var root = new VisualElement();

            var loadFakeEmotesButton = new Button(LoadFakeEmotes)
            {
                text = "Load Fake Emotes"
            };
            
            
            root.Add(loadFakeEmotesButton);

            return root;
        }

        private void LoadFakeEmotes()
        {
            var emotes = new List<string>();

            for (int i = 0; i < 8; i++)
                emotes.Add($"emote_{i}");
            
            Target.LoadEmoteData(emotes.ToArray());
        }
    }
}