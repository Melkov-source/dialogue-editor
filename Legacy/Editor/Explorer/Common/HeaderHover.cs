using History.Editor.HistoryEditor.Interfaces;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Common
{
    public class HeaderHover : IHandlerVisualElement
    {
        public VisualElement Root { get; private set; }

        private readonly VisualElement _header;
        
        private const string HEADER_SWAP_UP_ACTION_CLASS = "header-swap-up";
        private const string HEADER_SWAP_DOWN_ACTION_CLASS = "header-swap-down";

        private string _type;
        
        public HeaderHover(VisualElement header, VisualElement hover, string type)
        {
            Root = hover;
            _header = header;

            _type = type;
        }

        public void SetLine()
        {
            if (_type == "up")
            {
                SetSwapLineUp();
            }
            else
            {
                SetSwapLineDown();
            }
        }

        public void UnsetLine()
        {
            if (_type == "up")
            {
                UnsetSwapLineUp();
            }
            else
            {
                UnsetSwapLineDown();
            }
        }
        
        private void SetSwapLineUp()
        {
            _header.AddToClassList(HEADER_SWAP_UP_ACTION_CLASS);
        }

        private void SetSwapLineDown()
        {
            _header.AddToClassList(HEADER_SWAP_DOWN_ACTION_CLASS);
        }
        
        private void UnsetSwapLineUp()
        {
            _header.RemoveFromClassList(HEADER_SWAP_UP_ACTION_CLASS);
        }

        private void UnsetSwapLineDown()
        {
            _header.RemoveFromClassList(HEADER_SWAP_DOWN_ACTION_CLASS);
        }
    }
}