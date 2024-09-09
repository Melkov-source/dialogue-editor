using System;

namespace History.Editor.UIToolkitExtensions
{
    public class USSPathAttribute : Attribute
    {
        public readonly string Path;

        public USSPathAttribute(string path)
        {
            Path = path;
        }
    }
}