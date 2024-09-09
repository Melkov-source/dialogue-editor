using System;
using System.Collections.Generic;
using System.Reflection;
using History.Editor.HistoryEditor.Dialogue.Reflections;
using History.Editor.UIToolkitExtensions;
using History.Tools;
using Sandbox.Dialogue;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Nodes.Abstracts
{
    public abstract class DialogNodeEditorBase
    {
        public readonly VisualElement Root;
        public readonly DialogNodeBase Data;
        public readonly List<DialogNodeEditorBase> Nodes = new();

        public event Action<DialogNodeEditorBase> OnDelete;

        protected readonly DialogueSceneContext Context;

        private const string CONTENT_CLASS = "content";
        private const string HEADER_CLASS = "header";

        private const string FOLDOUT_BUTTON = "foldout-button";
        
        private const string ICON_CLASS = "icon-element";
        private const string NAME_CLASS = "child-name";

        private const string SELECT_CLASS = "select";

        private readonly VisualElement _header;
        private readonly VisualElement _content;

        private readonly Button _foldoutButton;
        private readonly VisualElement _icon;
        private readonly Label _nameLabel;

        private DisplayStyle _contentDisplayStyle = DisplayStyle.Flex;
        private readonly Sprite _arrowRightSprite;
        private readonly Sprite _arrowDownSprite;
        
        public DialogNodeEditorBase(DialogNodeBase data, DialogueSceneContext context)
        {
            Data = data;
            Context = context;
            Root = TemplateLoader.Get(TemplateType.DialogNodeTemplate);

            data.OnUpdateCharacterSpeaker += UpdateName;
            data.OnUpdateSideSpeaker += UpdateName;

            _arrowDownSprite = SpriteProvider.Get(SpriteType.arrow_down_icon);
            _arrowRightSprite = SpriteProvider.Get(SpriteType.arrow_right_icon);

            var style = StyleLoader.Get(StyleType.DialogNodeStyles);
            Root.styleSheets.Add(style);

            _header = Root.Q<VisualElement>(className: HEADER_CLASS);
            _content = Root.Q<VisualElement>(className: CONTENT_CLASS);
            
            _icon = _header.Q<VisualElement>(className: ICON_CLASS);
            _nameLabel = _header.Q<Label>(className: NAME_CLASS);

            _foldoutButton = Root.Q<Button>(className: FOLDOUT_BUTTON);
            _foldoutButton.clicked += OnFoldoutContentHandle;
            
            _header.RegisterCallback<MouseDownEvent>(OnOneClickHeaderHandle);
            
            CreateContextMenu();

            SetFoldoutContent(_contentDisplayStyle);
            
            if (Nodes.Count == 0)
            {
                SetActiveFoldoutButton(DisplayStyle.None);
            }
        }

        protected virtual void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Delete", OnDeleteNodeHandle);
        }
        

        public void SetIcon(Sprite icon)
        {
            _icon.style.backgroundImage = new StyleBackground(icon);
        }

        public void SetName(string name)
        {
            _nameLabel.text = name.Replace("\n", " <color=#8EB8E5>[NEW LINE]</color> ");
        }

        public virtual void UpdateName()
        {
            var targetName = "";

            if (HistoryEditorWindow.IsShowCharacterDialog)
            {
                var character = Context.GetCharacter(Data.CharacterIdSpeaker);

                if (character != default)
                {
                    targetName += $"<color=#5FAD56>[{character.Name}]</color> ";
                }
            }
            
            if (HistoryEditorWindow.IsShowSideDialog)
            {
                var colorMeta = DialogueSideColorMetaCacher.ParseColorMeta(Data.SideSpeaker);
                
                targetName += $"<color=#{colorMeta.HEX}>[{Data.SideSpeaker}]</color> ";
            }

            var sentence = Data as SentenceDialogNode;

            var sentenceText = sentence?.Sentence ?? "################";
            
            targetName += sentenceText;

            _header.tooltip = sentenceText.Replace("\n", " <color=#8EB8E5>[NEW LINE]</color>\n");
            
            SetName(targetName);
        }

        public virtual void AddNode(DialogNodeEditorBase node)
        {
            _content.Add(node.Root);
            Nodes.Add(node);

            node.OnDelete += DeleteNode;

            SetActiveFoldoutButton(DisplayStyle.Flex);
        }

        public virtual void DeleteNode(DialogNodeEditorBase node)
        {
            _content.Remove(node.Root);
            Nodes.Remove(node);

            if (Nodes.Count == 0)
            {
                SetActiveFoldoutButton(DisplayStyle.None);
            }
        }

        public virtual void Select()
        {
            _header.AddToClassList(SELECT_CLASS);
        }

        public virtual void Unselect()
        {
            _header.RemoveFromClassList(SELECT_CLASS);
        }
        
        protected virtual void OnOneClickHeaderHandle()
        {
            Context.Select(this);
        }

        protected virtual void OnDeleteNodeHandle(DropdownMenuAction action)
        {
            OnDelete?.Invoke(this);
        }
        
        protected void SetFoldoutContent(DisplayStyle displayStyle)
        {
            _contentDisplayStyle = displayStyle;
            _content.style.display = new StyleEnum<DisplayStyle>(_contentDisplayStyle);

            if (displayStyle == DisplayStyle.None)
            {
                _foldoutButton.style.backgroundImage = new StyleBackground(_arrowRightSprite);
            }
            else
            {
                _foldoutButton.style.backgroundImage = new StyleBackground(_arrowDownSprite);
            }
        }
        
        private void OnOneClickHeaderHandle(MouseDownEvent @event)
        {
            if (@event.button == 0)
            {
                OnOneClickHeaderHandle();
            }
        }
        
        private void CreateContextMenu()
        {
            var contextMenu = new ContextualMenuManipulator(ContextMenuBuild);

            _header.AddManipulator(contextMenu);
        }
        
        private void OnFoldoutContentHandle()
        {
            _contentDisplayStyle = _contentDisplayStyle == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
            
            SetFoldoutContent(_contentDisplayStyle);
        }

        private void SetActiveFoldoutButton(DisplayStyle displayStyle)
        {
            _foldoutButton.style.display = new StyleEnum<DisplayStyle>(displayStyle);
        }
    }
}