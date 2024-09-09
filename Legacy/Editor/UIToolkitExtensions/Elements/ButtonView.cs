using System;
using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public class ButtonView : EditorView
    {
        public event Action OnClickEvent;
        
        private readonly Button _button;
        
        public ButtonView(VisualElement visualElement) : base(visualElement)
        {
            _button = visualElement.Q<Button>();
            _button.RegisterCallback<ClickEvent>(OnClickHandle);
        }

        public void SetText(string text)
        {
            _button.text = text;
        }
        
        private void OnClickHandle(ClickEvent @event)
        {
            OnClickEvent?.Invoke();
        }

        public override void Dispose()
        {
            _button.UnregisterCallback<ClickEvent>(OnClickHandle);
        }
    }
}