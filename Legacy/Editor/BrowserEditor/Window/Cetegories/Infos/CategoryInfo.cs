using System.Collections.Generic;

namespace History.Editor.BrowserEditor.Window
{
    public class CategoryInfo
    {
        public string Name;
        public List<CategoryChildInfo> Children = new();
    }
}