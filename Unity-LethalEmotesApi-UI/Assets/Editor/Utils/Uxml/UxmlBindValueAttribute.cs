using System;

namespace Editor.Utils.Uxml
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UxmlBindValueAttribute : Attribute
    {
        public string BindPath { get; }
        
        public UxmlBindValueAttribute(string bindPath)
        {
            BindPath = bindPath;
        }
    }
}