using System;

namespace History.Editor.UIToolkitExtensions
{
    public class SpritePathAttribute : Attribute
    {
        public string Path;

        public SpritePathAttribute(string path)
        {
            Path = path;
        }
    }
}