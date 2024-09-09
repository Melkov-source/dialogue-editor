using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public class LabelView : EditorView
    {
        private readonly Label _label;
        
        public LabelView(VisualElement visualElement) : base(visualElement)
        {
            _label = visualElement.Q<Label>();
        }

        public void SetText(string text)
        {
            _label.text = text;
        }

        public string GetText(string text)
        {
            return _label.text;
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}