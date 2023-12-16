using LethalEmotesApi.Ui.DebugUtils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(EmoteWheelSegmentDebug))]
    public class EmoteWheelSegmentDebugEditor : UnityEditor.Editor 
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            var debug = (EmoteWheelSegmentDebug)target;
            var segment = debug.Target;

            var targetRect = segment.segmentRectTransform!;
            var targetWorldPos = new Vector3Field("Target World Pos")
            {
                value = targetRect.position,
            };
            var targetLocalPos = new Vector3Field("Target Local Pos")
            {
                value = targetRect.localPosition,
            };
            
            root.Add(targetWorldPos);
            root.Add(targetLocalPos);
            return root;
        }
    }
}