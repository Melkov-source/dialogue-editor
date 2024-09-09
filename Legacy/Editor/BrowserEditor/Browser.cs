using System;
using System.Collections.Generic;
using History.Editor.BrowserEditor.Window;
using History.Editor.BrowserEditor.Window.Enums;

namespace History.Editor.BrowserEditor
{
    public static class Browser
    {
        private static BrowserEditorWindow _window;

        private static Action<CategoryChildInfo> _onSelectAction;

        public static void OpenCategories(List<CategoryInfo> categories, Action<CategoryChildInfo> onSelectAction)
        {
            _window = BrowserEditorWindow.Open(BrowserMode.Categories, categories);

            _onSelectAction = onSelectAction;

            _window.OnSelectCategoryChildInfo += OnSelectHandle;
        }

        private static void OnSelectHandle(CategoryChildInfo categoryChildInfo)
        {
            _onSelectAction?.Invoke(categoryChildInfo);
            
            _window.Close();
        }
    }
}