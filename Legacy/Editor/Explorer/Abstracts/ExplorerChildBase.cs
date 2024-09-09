using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Fileinfo;
using History.Editor.HistoryEditor.Common;
using History.Tools.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public abstract class ExplorerChildBase
    {
        public abstract int Order { get; }
        public readonly Dictionary<VisualElement, ExplorerChildBase> Children;

        public GUID AssetId { get; }
        public abstract ExplorerEntityType ExplorerType { get; }
        public ExplorerChildInfo ChildInfo { get; }

        public string Name => AssetDatabaseUtilsEditor.GetName(AssetId);

        public event Action<ExplorerChildBase> OnDeleted;

        public VisualElement Content { get; private set; }
        public VisualElement Header { get; private set; }
        public HeaderHover HeaderUpHover { get; private set; }
        public HeaderHover HeaderDownHover { get; private set; }

        private const string HEADER_SELECTED_ACTION_CLASS = "header-selected";
        private const string HEADER_UN_SELECTED_ACTION_CLASS = "header-unselected";
        private const string HEADER_DROP_ACTION_CLASS = "header-drop";
        
        private const string CONTENT_CLASS = "content";
        private const string HEADER_CLASS = "header";
        
        private const string HEADER_UP_HOVER_CLASS = "drop-up-hover";
        private const string HEADER_DOWN_HOVER_CLASS = "drop-down-hover";

        private const string ICON_CLASS = "icon-element";
        private const string NAME_CLASS = "child-name";
        private const string RENAME_TEXT_FIELD_CLASS = "rename-text-field";

        private readonly Button _foldoutButton;
        private readonly VisualElement _icon;
        private readonly Label _nameLabel;
        private readonly TextField _renameTextField;


        protected ExplorerChildBase(GUID assetId, ExplorerChildInfo childInfo)
        {
            AssetId = assetId;
            ChildInfo = childInfo;

            Content = ChildInfo.Root.Q<VisualElement>(className: CONTENT_CLASS);
            Header = ChildInfo.Root.Q<VisualElement>(className: HEADER_CLASS);

            var headerHoverUp = ChildInfo.Root.Q<VisualElement>(className: HEADER_UP_HOVER_CLASS);
            var headerHoverDown = ChildInfo.Root.Q<VisualElement>(className: HEADER_DOWN_HOVER_CLASS);
            
            HeaderUpHover = new HeaderHover(Header, headerHoverUp, "up");
            HeaderDownHover = new HeaderHover(Header, headerHoverDown, "down");

            _icon = Header.Q<VisualElement>(className: ICON_CLASS);
            _nameLabel = Header.Q<Label>(className: NAME_CLASS);
            _renameTextField = Header.Q<TextField>(className: RENAME_TEXT_FIELD_CLASS);

            Header.RegisterCallback<MouseDownEvent>(OnOneClickHeaderHandle);

            _renameTextField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.character == '\n')
                {
                    evt.StopPropagation();
                    evt.PreventDefault();

                    SetState(CHILD_STATE.NONE);
                    SetName(_nameLabel.text);

                    if (ExplorerType == ExplorerEntityType.File)
                    {
                        Explorer.UpdateFile(AssetId);
                    }
                }
            }, TrickleDown.TrickleDown);

            _renameTextField.RegisterCallback<ChangeEvent<string>>((evt) => { _nameLabel.text = evt.newValue; });

            SetState(CHILD_STATE.NONE);

            CreateContextMenu();

            Children = new Dictionary<VisualElement, ExplorerChildBase>();

            UnSelect();

            _nameLabel.text = AssetDatabaseUtilsEditor.GetName(AssetId);
        }

        public void Select()
        {
            Header.AddToClassList(HEADER_SELECTED_ACTION_CLASS);

            Header.RemoveFromClassList(HEADER_UN_SELECTED_ACTION_CLASS);
        }

        public void UnSelect()
        {
            Header.AddToClassList(HEADER_UN_SELECTED_ACTION_CLASS);

            Header.RemoveFromClassList(HEADER_SELECTED_ACTION_CLASS);
        }

        public void SetDropLine()
        {
            Header.AddToClassList(HEADER_DROP_ACTION_CLASS);
        }

        public void UnsetDropLine()
        {
            Header.RemoveFromClassList(HEADER_DROP_ACTION_CLASS);
        }

        public void SetName(string name)
        {
            AssetDatabaseUtilsEditor.Rename(AssetId, name);
        }

        public void SetIcon(Sprite icon)
        {
            _icon.style.backgroundImage = new StyleBackground(icon);
        }

        protected virtual void OnOneClickHeaderHandle()
        {
        }

        protected virtual void AddChild(ExplorerChildBase child)
        {
            child.OnDeleted += OnDeletedChildHandle;

            Children.Add(child.ChildInfo.Root, child);
            Content.Add(child.ChildInfo.Root);

            child.ChildInfo.Parent = this;

            Content.Sort((a, b) =>
            {
                var child_1 = Children[a];
                var child_2 = Children[b];

                return child_1.Order - child_2.Order;
            });
        }

        protected virtual void OnDeletedChildHandle(ExplorerChildBase child)
        {
            child.OnDeleted -= OnDeletedChildHandle;

            var root = child.ChildInfo.Root;

            Children.Remove(root);
            Content.Remove(root);
        }

        protected virtual void OnDeleteHandle(DropdownMenuAction action)
        {
            ChildInfo.Root.RemoveFromHierarchy();

            Explorer.DeleteAsset(AssetId);

            OnDeleted?.Invoke(this);
        }

        protected virtual void OnRenameHandle(DropdownMenuAction action)
        {
            SetState(CHILD_STATE.RENAME);
        }

        protected virtual void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Delete", OnDeleteHandle);
            @event.menu.AppendAction("Rename", OnRenameHandle);
            @event.menu.AppendAction("Show in File Explorer", OnShowInFileExplorer);
        }

        private void OnOneClickHeaderHandle(MouseDownEvent @event)
        {
            if (@event.button == 0)
            {
                OnOneClickHeaderHandle();
            }
        }

        private void SetState(CHILD_STATE state)
        {
            switch (state)
            {
                case CHILD_STATE.NONE:
                    _renameTextField.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _nameLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    break;
                case CHILD_STATE.RENAME:
                    _renameTextField.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _nameLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                    _renameTextField.Focus();
                    break;
            }
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextualMenuManipulator(ContextMenuBuild);

            Header.AddManipulator(contextMenu);
        }

        private void OnShowInFileExplorer(DropdownMenuAction action)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(AssetId);

            var folderPath = System.IO.Path.GetDirectoryName(assetPath);

            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }

        private enum CHILD_STATE : byte
        {
            NONE = 0,
            RENAME = 1
        }
    }
}