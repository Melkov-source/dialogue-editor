using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class SceneTabMenu
    {
        public readonly VisualElement Root;

        public event Action<GUID> OnCloseTab;
        public event Action<GUID> OnSelectTab;

        private readonly Dictionary<GUID, SceneTab> _tabs = new();
        
        public SceneTabMenu(VisualElement root)
        {
            Root = root;
        }

        public void SelectFile(GUID assetId)
        {
            foreach (var concreteTab in _tabs.Values)
            {
                if (concreteTab.AssetId == assetId)
                {
                    concreteTab.Select();
                }
                else
                {
                    concreteTab.Unselect();
                }
            }
        }
        
        public void UpdateTab(GUID assetId)
        {
            if (_tabs.TryGetValue(assetId, out var tab))
            {
                tab.UpdateView();
            }
        }

        public void AddTab(GUID assetId)
        {
            var tab = new SceneTab(assetId);
            
            tab.OnSelect += OnSelectTabHandle;
            tab.OnClose += OnCloseTabHandle;

            _tabs.Add(assetId, tab);
            Root.Add(tab.Root);

            SelectFile(assetId);
        }

        private void OnCloseTabHandle(SceneTab tab)
        {
            RemoveTab(tab.AssetId);
            
            OnCloseTab?.Invoke(tab.AssetId);
        }

        private void OnSelectTabHandle(SceneTab tab)
        {
            var assetId = tab.AssetId;
            
            SelectFile(assetId);
            
            Explorer.SelectFile(assetId);
            
            OnSelectTab?.Invoke(assetId);
        }

        public void RemoveTab(GUID assetId)
        {
            if (_tabs.TryGetValue(assetId, out var tab))
            {
                tab.Root.RemoveFromHierarchy();
                _tabs.Remove(assetId);
            }
        }
    }
}