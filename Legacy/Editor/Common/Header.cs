using System;
using History.Editor.UIToolkitExtensions;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class Header : IDisposable
    {
        public event Action OnPin;
        public event Action OnClose;

        public const string ROOT_CLASS = "header";
        
        private const string TITLE_LABEL_CLASS = "title-label";
        
        private const string CLOSE_BUTTON_CLASS = "header-close-button";
        private const string PIN_BUTTON_CLASS = "header-pin-button";
        
        private readonly VisualElement _root;

        private readonly Label _titleLabel;

        private readonly Button _closeButton;
        private readonly Button _pinButton;
        
        public Header(VisualElement root)
        {
            var style = StyleLoader.Get(StyleType.HeaderStyles);
            root.styleSheets.Add(style);
            
            _root = root;

            _titleLabel = root.Q<Label>(className: TITLE_LABEL_CLASS);
            
            _closeButton = root.Q<Button>(className: CLOSE_BUTTON_CLASS);
            _pinButton = root.Q<Button>(className: PIN_BUTTON_CLASS);

            _closeButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _pinButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            _closeButton.focusable = true;

            _closeButton.clicked += OnCloseHandle;
            _pinButton.clicked += OnPinHandle;
        }
        
        public void Dispose()
        {
            _closeButton.clicked -= OnCloseHandle;
            _pinButton.clicked -= OnPinHandle;
        }

        public void SetTitle(string title)
        {
            _titleLabel.text = title;
        }

        private void OnPinHandle()
        {
            OnPin?.Invoke();
        }

        private void OnCloseHandle()
        {
            OnClose?.Invoke();
        }
    }
}