using History.Editor.UIToolkitExtensions;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Abstracts
{
    public abstract class PropertiesGroupBase
    {
        public VisualElement Root { get; protected set; }

        public PropertiesGroupBase(TemplateType templateType)
        {
            Root = TemplateLoader.Get(templateType);
            
            var style = StyleLoader.Get(StyleType.DialogNodePropertiesStyles);
            Root.styleSheets.Add(style);
        }

        public virtual void Open()
        {
            Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public virtual void Close()
        {
            Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
    }
}