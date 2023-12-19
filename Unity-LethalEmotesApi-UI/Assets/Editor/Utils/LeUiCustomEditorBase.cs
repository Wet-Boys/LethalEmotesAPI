using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Utils
{
    public abstract class LeUiCustomEditorBase<TTarget> : UnityEditor.Editor
        where TTarget : Object
    {
        protected TTarget Target => (TTarget)target;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            root.Add(CreateDefaultGUI());
            
            root.Add(CreateGUI());

            return root;
        }

        protected abstract VisualElement CreateGUI();

        private VisualElement CreateDefaultGUI()
        {
            var container = new VisualElement();

            var iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                do
                {
                    var propField = new PropertyField(iterator.Copy())
                    {
                        name = $"PropertyField: {iterator.propertyPath}"
                    };

                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                        propField.SetEnabled(false);
                    
                    container.Add(propField);
                    
                } while (iterator.NextVisible(false));
            }

            return container;
        }
    }
}