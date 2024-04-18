using System;

namespace Editor.Utils.Uxml
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UxmlOnBindElementAttribute : Attribute
    {
        public string BindPath { get; }

        public UxmlOnBindElementAttribute(string bindPath)
        {
            BindPath = bindPath;
        }
    }
}