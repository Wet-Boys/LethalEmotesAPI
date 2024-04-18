using System;

namespace Editor.Utils.Uxml
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UxmlBindButtonAttribute : Attribute
    {
        public string BindPath { get; }

        public UxmlBindButtonAttribute(string bindPath)
        {
            BindPath = bindPath;
        }
    }
}