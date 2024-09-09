using System;
using History.Editor.UIToolkitExtensions;
using History.Tools.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class SceneTab
    {
        public VisualElement Root;
        
        public event Action<SceneTab> OnSelect;
        public event Action<SceneTab> OnClose;
        
        public GUID AssetId { get; }

        private const string NAME_LABEL_CLASS = "name-label";
        private const string CLOSE_BUTTON_CLASS = "close-button";
        
        private const string SELECT_ACTION_CLASS = "selected";
        private const string UNSELECT_ACTION_CLASS = "unselected";

        private readonly Label _nameLabel;
        private readonly Button _closeButton;
        
        public SceneTab(GUID assetId)
        {
            AssetId = assetId;

            //TODO:
            //fileInfo.OnChangeName += () => SetName(fileInfo.FileSystemInfo.Name);
            
            Root = TemplateLoader.Get(TemplateType.SceneTabMenuChildTemplate);
            var style = StyleLoader.Get(StyleType.SceneTabStyles);
            
            Root.styleSheets.Add(style);

            _nameLabel = Root.Q<Label>(className: NAME_LABEL_CLASS);
            _closeButton = Root.Q<Button>(className: CLOSE_BUTTON_CLASS);

            _closeButton.clicked += OnCloseHandle;
            
            Root.RegisterCallback<MouseDownEvent>(OnSelectHandle);

            UpdateView();
        }
        
        public void UpdateView()
        {
            var name = AssetDatabaseUtilsEditor.GetName(AssetId);
            
            SetName(name);
        }

        public void SetName(string name)
        {
            _nameLabel.text = name;
        }

        public void Select()
        {
            Root.AddToClassList(SELECT_ACTION_CLASS);
            Root.RemoveFromClassList(UNSELECT_ACTION_CLASS);
        }

        public void Unselect()
        {
            Root.RemoveFromClassList(SELECT_ACTION_CLASS);
            Root.AddToClassList(UNSELECT_ACTION_CLASS);
        }

        private void OnCloseHandle()
        {
            Debug.Log("OnClose");
            OnClose?.Invoke(this);
        }
        
        private void OnSelectHandle(MouseDownEvent @event)
        {
            OnSelect?.Invoke(this);
        }
    }
}