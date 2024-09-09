using System;
using System.IO;
using History.Editor.UIToolkitExtensions;
using History.Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.FileSelectorEditor.Elements
{
    public class FileCellElement
    {
        public readonly VisualElement Root;

        public event Action<FileInfo> OnClick;

        private const string ICON_CLASS = "icon";
        private const string NAME_CLASS = "name-label";

        private readonly VisualElement _iconElement;
        private readonly ScrollView _scrollView;

        private readonly FileInfo _fileInfo;

        private bool _isInitialized;
        private bool _isVisible;

        public FileCellElement(FileInfo fileInfo, ScrollView scrollView)
        {
            _fileInfo = fileInfo;
            
            Root = TemplateLoader.Get(TemplateType.FileCellTemplate);
            _scrollView = scrollView;
            
            Root.RegisterCallback<ClickEvent>((@event) => OnClickHandle());
            
            _iconElement = Root.Q<VisualElement>(className: ICON_CLASS);
            var nameLabel = Root.Q<Label>(className: NAME_CLASS);

           
            nameLabel.text = Path.GetFileNameWithoutExtension(fileInfo.FullName);
        }

        public void Update()
        {
            var scrollViewWorldBound = _scrollView.worldBound;
            var viewportWorldBound = _scrollView.contentViewport.worldBound;

            var elementWorldBound = Root.worldBound;
            _isVisible = scrollViewWorldBound.Overlaps(elementWorldBound) && viewportWorldBound.Contains(elementWorldBound.min) && viewportWorldBound.Contains(elementWorldBound.max);

            var display = _isVisible ? DisplayStyle.Flex : DisplayStyle.None;

            _iconElement.style.display = new StyleEnum<DisplayStyle>(display);

            if (_isVisible && _isInitialized == false)
            {
                _isInitialized = true;
                
                var picture = _fileInfo.CreateTexture();

#pragma warning disable CS0618 // Type or member is obsolete
                _iconElement.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit);
#pragma warning restore CS0618 // Type or member is obsolete
                _iconElement.style.backgroundImage = new StyleBackground(picture);
            }
        }

        public void Unselect()
        {
           Root.RemoveFromClassList("select");
        }

        public void Select()
        {
            Root.AddToClassList("select");
        }
        
        private void OnClickHandle()
        {
            OnClick?.Invoke(_fileInfo);
        }
    }
}