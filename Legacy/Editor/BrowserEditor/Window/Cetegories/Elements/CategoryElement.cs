using System;
using System.Collections.Generic;
using System.Linq;
using History.Editor.UIToolkitExtensions;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEngine.UIElements;

namespace History.Editor.BrowserEditor.Window
{
    public class CategoryElement
    {
        public readonly VisualElement Root;

        public event Action<CategoryInfo, CategoryChildInfo> OnSelect;
        
        private const string HEADER_NAME_LABEL_CLASS = "header-name";
        private const string CONTENT_CLASS = "content";

        private readonly Label _nameLabel;
        private readonly VisualElement _content;

        private readonly Dictionary<CategoryChildInfo, CategoryChildElement> _children = new();

        private readonly CategoryInfo _info;
        
        public CategoryElement(CategoryInfo info)
        {
            _info = info;
            
            Root = TemplateLoader.Get(TemplateType.CategotyElementTemplate);

            _nameLabel = Root.Q<Label>(className: HEADER_NAME_LABEL_CLASS);
            _content = Root.Q<VisualElement>(className: CONTENT_CLASS);

            _nameLabel.text = info.Name;
            
            for (int index = 0, count = info.Children.Count; index < count; index++)
            {
                var childInfo = info.Children[index];

                CreateChildView(childInfo);
            }
        }

        public void UnselectChildren()
        {
            foreach (var childElement in _children.Values)
            {
                childElement.Unselect();
            }
        }
        
        private void CreateChildView(CategoryChildInfo info)
        {
            var view = new CategoryChildElement(info);

            view.OnSelect += OnSelectCategoryElementHandle;
            
            _content.Add(view.Root);
            
            _children.Add(info, view);
        }

        private void OnSelectCategoryElementHandle(CategoryChildInfo childInfo)
        {
            var element = _children[childInfo];
            
            element.Select();
            
            OnSelect?.Invoke(_info, childInfo);
        }
    }
}