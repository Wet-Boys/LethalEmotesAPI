using LethalEmotesApi.Ui;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EmoteWheelsController))]
    public class EmoteWheelsControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var controller = (EmoteWheelsController)target;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Prev"))
            {
                controller.PrevWheel();
            }
            else if (GUILayout.Button("Next"))
            {
                controller.NextWheel();
            }
            
            GUILayout.EndHorizontal();
        }
    }
}