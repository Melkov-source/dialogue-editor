using System;

namespace Sandbox.Dialogue
{
    public class DialogParameterMetaAttribute : Attribute
    {
        public string Icon = "error_icon";
        public string Category = "Default";
        public string Name = "Not found name!";
        public string Description = "Not found description!";
        
        public DialogParameterType Type;
        public object ConcreteType;
    }
}