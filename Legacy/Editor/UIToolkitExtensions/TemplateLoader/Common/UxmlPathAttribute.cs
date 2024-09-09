using System;

namespace History.Editor.UIToolkitExtensions
{
    public class UxmlPathAttribute : Attribute
    {
        public readonly string Path;

        public UxmlPathAttribute(string path)
        {
            Path = path;
        }
    }
}