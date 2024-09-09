using System.Linq;
using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public class EditorTemplate
    {
        private readonly VisualTreeAsset _visualTreeAsset;

        public EditorTemplate(VisualTreeAsset visualTreeAsset)
        {
            _visualTreeAsset = visualTreeAsset;
        }

        public VisualElement Create()
        {
            var visualElement = _visualTreeAsset.Instantiate().Children().FirstOrDefault();
            return visualElement;
        }
    }
}