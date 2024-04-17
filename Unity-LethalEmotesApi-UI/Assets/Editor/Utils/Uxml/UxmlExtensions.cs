using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

namespace Editor.Utils.Uxml
{
    public static class UxmlExtensions
    {
        public static VisualElement QueryPath(this VisualElement root, string path)
        {
            var selectors = path.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (selectors.Length == 0)
                return null;

            var element = root;
            foreach (var selector in selectors)
                element = element.Q(selector);

            return element;
        }

        public static T QueryPath<T>(this VisualElement root, string path)
            where T : VisualElement
        {
            return (T)root.QueryPath(path);
        }
        
        public static void BindFields(this VisualElement root, object instance)
        {
            var fieldInfoArray = instance.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var fieldInfo in fieldInfoArray)
            {
                var attr = fieldInfo.GetCustomAttribute(typeof(UxmlBindValueAttribute));
                if (attr is UxmlBindValueAttribute bindValueAttr)
                {
                    var element = root.QueryPath(bindValueAttr.BindPath);
                    if (element is null)
                    {
                        Debug.LogWarning($"Couldn't bind an element with path '{bindValueAttr.BindPath}'!");
                        continue;
                    }

                    var fieldType = fieldInfo.FieldType;

                    if (fieldType == typeof(string))
                        TryBindFieldValueListener<string>(fieldInfo, element, instance, true);
                    else if (fieldType == typeof(Color))
                        TryBindFieldValueListener<Color>(fieldInfo, element, instance);
                    else if (fieldType == typeof(bool))
                        TryBindFieldValueListener<bool>(fieldInfo, element, instance);
                    else if (fieldType == typeof(int))
                        TryBindFieldValueListener<int>(fieldInfo, element, instance);
                    else if (fieldType == typeof(GameObject))
                        TryBindFieldValueListener<Object>(fieldInfo, element, instance);
                }
                else if (attr is UxmlBindVisualElementAttribute bindElementAttr)
                {
                    var element = root.QueryPath(bindElementAttr.BindPath);
                    if (element is null)
                    {
                        Debug.LogWarning($"Couldn't bind an element with path '{bindElementAttr.BindPath}'!");
                        continue;
                    }
                    
                    fieldInfo.SetValue(instance, element);
                }
            }
        }
        
        private static void TryBindFieldValueListener<T>(FieldInfo fieldInfo, VisualElement element, object instance, bool setDefault = false)
        {
            if (element is not INotifyValueChanged<T> notifyValueChanged)
            {
                Debug.LogWarning($"{element.name} is not the correct INotifyValueChanged Type! Expected: '{typeof(T).Name}' Got: '{element.GetType()}'!");
                return;
            }

            if (setDefault)
                fieldInfo.SetValue(instance, default);

            notifyValueChanged.RegisterValueChangedCallback(evt => fieldInfo.SetValue(instance, evt.newValue));
        }

        public static void BindButtonMethods(this VisualElement root, object instance)
        {
            var methodInfoArray = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var methodInfo in methodInfoArray)
            {
                var attr = methodInfo.GetCustomAttribute(typeof(UxmlBindButtonAttribute));
                if (attr is not UxmlBindButtonAttribute bindButtonAttr)
                    continue;
                
                var button = root.QueryPath<Button>(bindButtonAttr.BindPath);
                if (button is null)
                {
                    Debug.LogWarning($"Couldn't bind an button with path '{bindButtonAttr.BindPath}'!");
                    continue;
                }
                
                button.RegisterCallback<MouseUpEvent>(_ => methodInfo.Invoke(instance, Array.Empty<object>()));
            }
        }
        
        public static void BindOnBindListeners(this VisualElement root, object instance)
        {
            var methodInfoArray = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (var methodInfo in methodInfoArray)
            {
                var attr = methodInfo.GetCustomAttribute(typeof(UxmlOnBindElementAttribute));
                if (attr is not UxmlOnBindElementAttribute onBindAttr)
                    continue;
                
                var element = root.QueryPath(onBindAttr.BindPath);
                if (element is null)
                {
                    Debug.LogWarning($"Couldn't find an element with path '{onBindAttr.BindPath}'!");
                    continue;
                }

                methodInfo.Invoke(instance, new object[] { element });
            }
        }
        
        public static void BindOnBindListeners(this VisualElement root, object instance, SerializedProperty property)
        {
            var methodInfoArray = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (var methodInfo in methodInfoArray)
            {
                var attr = methodInfo.GetCustomAttribute(typeof(UxmlOnBindElementAttribute));
                if (attr is not UxmlOnBindElementAttribute onBindAttr)
                    continue;
                
                var element = root.QueryPath(onBindAttr.BindPath);
                if (element is null)
                {
                    Debug.LogWarning($"Couldn't find an element with path '{onBindAttr.BindPath}'!");
                    continue;
                }

                methodInfo.Invoke(instance, new object[] { element, property });
            }
        }
    }
}