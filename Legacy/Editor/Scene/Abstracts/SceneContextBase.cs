using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public abstract class SceneContextBase : IDisposable
    {
        public readonly GUID AssetId;
        
        public abstract VisualElement Root { get; }

        public SceneContextBase(GUID assetId)
        {
            AssetId = assetId;
        }
        
        public virtual void Show()
        {
            Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public virtual void Hide()
        {
            Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
        
        public virtual void Dispose()
        {
            // TODO release managed resources here
        }

        public virtual void Save()
        {
            
        }
    }
}