using System;

namespace History.Editor.HistoryEditor.Reflections
{
    public class FileExtensionAttribute : Attribute
    {
        public readonly string Extension;

        public FileExtensionAttribute(string extension)
        {
            Extension = extension;
        }
    }
}