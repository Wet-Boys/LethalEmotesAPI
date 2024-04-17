using System;

namespace Editor.Utils.Uxml
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UxmlBindVisualElementAttribute : Attribute
    {
        public string BindPath { get; }
        
        public UxmlBindVisualElementAttribute(string bindPath)
        {
            BindPath = bindPath;
        }
    }
}