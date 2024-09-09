using System;
using System.Collections.Generic;
using System.IO;
using History.Editor.UIToolkitExtensions;
using Newtonsoft.Json;
using Sandbox.Scripts.Blackboards;
using UnityEditor;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Blackboards
{
    public class BlackboardSceneContext : SceneContextBase
    {
        public sealed override VisualElement Root { get; }

        private const string SCROLL_VIEW_CLASS = "blackboard-scroll-view";
        
        private readonly List<BlackboardObjectElement> _dataElements = new();
        private readonly ScrollView _scrollView;

        private readonly Blackboard _blackboard;
        
        public BlackboardSceneContext(Blackboard blackboard, GUID assetId) : base(assetId)
        {
            _blackboard = blackboard;

            Root = TemplateLoader.Get(TemplateType.BlackboardSceneContext);

            var style = StyleLoader.Get(StyleType.BlackboardSceneContextStyles);
            Root.styleSheets.Add(style);

            _scrollView = Root.Q<ScrollView>(className: SCROLL_VIEW_CLASS);

            var data = _blackboard.GetAllDataObjects();

            for (int index = 0, count = data.Count; index < count; index++)
            {
                var dataObject = data[index];

                CreateDataObjectElementView(dataObject);
            }

            CreateContextMenu();
        }

        public override void Save()
        {
            base.Save();
            
            var filePath = AssetDatabase.GUIDToAssetPath(AssetId);

            var json = JsonConvert.SerializeObject(_blackboard, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText(filePath, json);
        }
        
        private void CreateContextMenu()
        {
            var contextMenu = new ContextualMenuManipulator(ContextMenuBuild);

            Root.AddManipulator(contextMenu);
        }

        private void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Create/Data", OnCreateNewDataObject);
        }

        private void OnCreateNewDataObject(DropdownMenuAction action)
        {
            var id = Guid.NewGuid();
            var dataObject = new DataObject(id);
            
            _blackboard.AddDataObject(dataObject);

            CreateDataObjectElementView(dataObject);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }

        private void CreateDataObjectElementView(DataObject dataObject)
        {
            var element = new BlackboardObjectElement(dataObject);
            _dataElements.Add(element);
            
            _scrollView.Add(element.Root);
        }
    }
}