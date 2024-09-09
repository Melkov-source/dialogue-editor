using System;
using History.Editor.UIToolkitExtensions;
using UnityEngine.UIElements;

namespace History.Editor.BrowserEditor.Window
{
    public class CategoryChildElement
    {
        public readonly VisualElement Root;

        public event Action<CategoryChildInfo> OnSelect;
        
        private const string ICON_CLASS = "icon";
        private const string NAME_CLASS = "name";

        private const string SELECT_ACTION_CLASS = "category-element-select";

        private readonly VisualElement _iconElement;
        private readonly Label _nameLabel;

        private readonly CategoryChildInfo _info;


        public CategoryChildElement(CategoryChildInfo info)
        {
            _info = info;
            
            Root = TemplateLoader.Get(TemplateType.CategotyChildElementTemplate);

            Root.tooltip = info.Description;

            _iconElement = Root.Q<VisualElement>(className: ICON_CLASS);
            _nameLabel = Root.Q<Label>(className: NAME_CLASS);

            _iconElement.style.backgroundImage = new StyleBackground(info.Icon);
            _nameLabel.text = info.Name;
            
            Root.RegisterCallback<ClickEvent>(OnClickHandle);
        }

        public void Select()
        {
            Root.AddToClassList(SELECT_ACTION_CLASS);
        }

        public void Unselect()
        {
            Root.RemoveFromClassList(SELECT_ACTION_CLASS);
        }

        private void OnClickHandle(ClickEvent evt)
        {
            OnSelect?.Invoke(_info);
        }
    }
}