using System;
using System.Collections.Generic;
using System.IO;
using History.Editor.HistoryEditor.Characters;
using History.Editor.HistoryEditor.Dialogue.Nodes;
using History.Editor.HistoryEditor.Dialogue.Nodes.Abstracts;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using Sandbox.Scripts.Characters;
using Sandbox.Scripts.Database.Enums;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue
{
    public class DialogueSceneContext : SceneContextBase
    {
        public sealed override VisualElement Root { get; }

        private const string SCROLL_VIEW_CONTENT_CLASS = "scroll-view-content";
        
        private const string CHARACTER_TOGGLE_CLASS = "dialog-character-toggle";
        private const string SIDE_TOGGLE_CLASS = "dialog-side-toggle";

        private readonly RootDialogNodeEditor _rootDialogNode;
        private readonly Toggle _characterToggle;
        private readonly Toggle _sideToggle;

        private DialogNodeEditorBase _currentSelectedNode;

        private readonly Dialog _dialog;

        private readonly Dictionary<Guid, CharacterInfo> _characters = new();

        public DialogueSceneContext(Dialog dialog, GUID assetId) 
            : base(assetId)
        {
            _dialog = dialog;
            
            var files = Explorer.GetFiles<CharacterExplorerFile>(DataType.CHARACTER);
            
            for (int index = 0, count = files.Count; index < count; index++)
            {
                var file = files[index];
                var character = (CharacterInfo)file.Read();
                
                _characters.Add(character.Id, character);
            }
            
            Root = TemplateLoader.Get(TemplateType.DialogueSceneContext);

            var style = StyleLoader.Get(StyleType.DialogueSceneContextStyles);
            Root.styleSheets.Add(style);

            var scrollViewContent = Root.Q<ScrollView>(className: SCROLL_VIEW_CONTENT_CLASS);
            
            _characterToggle = Root.Q<Toggle>(className: CHARACTER_TOGGLE_CLASS);
            _sideToggle = Root.Q<Toggle>(className: SIDE_TOGGLE_CLASS);

            _characterToggle.RegisterValueChangedCallback(OnChangedCharacterToggleHandle);
            _sideToggle.RegisterValueChangedCallback(OnChangedSideToggleHandle);
            
            _characterToggle.SetValueWithoutNotify(HistoryEditorWindow.IsShowCharacterDialog);
            _sideToggle.SetValueWithoutNotify(HistoryEditorWindow.IsShowSideDialog);

            _rootDialogNode = new RootDialogNodeEditor(dialog.Tree.Root, this);
            scrollViewContent.Add(_rootDialogNode.Root);

            _currentSelectedNode = _rootDialogNode;

            CreateContextMenu();
        }

        public CharacterInfo GetCharacter(Guid characterId)
        {
            if (_characters.TryGetValue(characterId, out var character))
            {
                return character;
            }

            return default;
        }

        private void OnChangedCharacterToggleHandle(ChangeEvent<bool> evt)
        {
            HistoryEditorWindow.IsShowCharacterDialog = evt.newValue;
            UpdateNameForNodes();
        }
        
        private void OnChangedSideToggleHandle(ChangeEvent<bool> evt)
        {
            HistoryEditorWindow.IsShowSideDialog = evt.newValue;
            UpdateNameForNodes();
        }


        private void UpdateNameForNodes()
        {
            var nodes = GetNodeChildren(_rootDialogNode);

            for (int index = 0, count = nodes.Count; index < count; index++)
            {
                var node = nodes[index];

                node.UpdateName();
            }
        }

        public override void Show()
        {
            base.Show();

            Select(_currentSelectedNode);
            
            _characterToggle.SetValueWithoutNotify(HistoryEditorWindow.IsShowCharacterDialog);
            _sideToggle.SetValueWithoutNotify(HistoryEditorWindow.IsShowSideDialog);
            
            UpdateNameForNodes();
        }

        public void Select(DialogNodeEditorBase targetNode)
        {
            var allNodes = GetNodeChildren(_rootDialogNode);

            for (int index = 0, count = allNodes.Count; index < count; index++)
            {
                var node = allNodes[index];

                if (node != targetNode)
                {
                    node.Unselect();
                }
            }
            
            targetNode.Select();

            _currentSelectedNode = targetNode;
        }
        
        public override void Save()
        {
            base.Save();

            var filePath = AssetDatabase.GUIDToAssetPath(AssetId);

            var json = JsonConvert.SerializeObject(_dialog, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText(filePath, json);
        }

        private List<DialogNodeEditorBase> GetNodeChildren(DialogNodeEditorBase targetNode)
        {
            var nodes = new List<DialogNodeEditorBase> { targetNode };

            for (int index = 0, count = targetNode.Nodes.Count; index < count; index++)
            {
                var node = targetNode.Nodes[index];

                var children = GetNodeChildren(node);
                
                nodes.Add(node);
                nodes.AddRange(children);
            }

            return nodes;
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextualMenuManipulator(ContextMenuBuild);

            Root.AddManipulator(contextMenu);
        }

        private void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Node/Sentence", CreateSentenceNodeHandle);
            @event.menu.AppendAction("Node/Choice", CreateChoiceNodeHandle);
        }

        private void CreateSentenceNodeHandle(DropdownMenuAction action)
        {
            var node = new SentenceDialogNode();
            var nodeView = new SentenceDialogNodeEditor(node, this);

            _rootDialogNode.AddNode(nodeView);
        }

        private void CreateChoiceNodeHandle(DropdownMenuAction action)
        {
            var node = new ChoiceDialogNode();
            var nodeView = new ChoiceDialogNodeEditor(node, this);

            _rootDialogNode.AddNode(nodeView);
        }
    }
}