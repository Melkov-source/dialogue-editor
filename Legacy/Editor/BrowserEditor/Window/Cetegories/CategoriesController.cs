using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace History.Editor.BrowserEditor.Window
{
    public class CategoriesController
    {
        public readonly ScrollView Root;

        public event Action<CategoryChildInfo> OnSelect; 

        private readonly Dictionary<CategoryInfo, CategoryElement> _categoryViews = new();
        
        public CategoriesController(ScrollView root)
        {
            Root = root;
        }

        public void Setup(List<CategoryInfo> categories)
        {
            Clear();

            for (int index = 0, count = categories.Count; index < count; index++)
            {
                var categoryInfo = categories[index];
                
                CreateCategoryView(categoryInfo);
            }
        }

        private void Clear()
        {
            foreach (var view in _categoryViews.Values)
            {
                Root.Remove(view.Root);
            }
            
            _categoryViews.Clear();
        }

        private void CreateCategoryView(CategoryInfo info)
        {
            var categoryView = new CategoryElement(info);

            categoryView.OnSelect += OnSelectHandle;
            
            Root.Add(categoryView.Root);
            
            _categoryViews.Add(info, categoryView);
        }

        private void OnSelectHandle(CategoryInfo categoryInfo, CategoryChildInfo categoryChildInfo)
        {
            OnSelect?.Invoke(categoryChildInfo);
            
            var targetView = _categoryViews[categoryInfo];
            
            foreach (var view in _categoryViews.Values.Where(view => view.Equals(targetView) == false))
            {
                view.UnselectChildren();
            }
        }
    }
}