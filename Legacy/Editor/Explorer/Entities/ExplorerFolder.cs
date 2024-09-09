using System;
using System.IO;
using History.Editor.HistoryEditor.Blackboards;
using History.Editor.HistoryEditor.Chapters;
using History.Editor.HistoryEditor.Characters;
using History.Editor.HistoryEditor.Dialogue;
using History.Editor.HistoryEditor.Items;
using History.Editor.HistoryEditor.Locations;
using History.Editor.HistoryEditor.Utils;
using History.Editor.UIToolkitExtensions;
using History.Tools.Editor;
using Sandbox.Scripts.Database.Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class ExplorerFolder : ExplorerChildBase
    {
        public override int Order => 0;
        public override ExplorerEntityType ExplorerType => ExplorerEntityType.Folder;

        private DisplayStyle _contentDisplayStyle;

        private readonly Sprite _emptyFolderIcon;
        private readonly Sprite _filesFolderIcon;

        public ExplorerFolder(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            _emptyFolderIcon = SpriteProvider.Get(SpriteType.folder_icon);
            _filesFolderIcon = SpriteProvider.Get(SpriteType.folder_with_files_icon);

            SetIcon(_emptyFolderIcon);

            LoadFolder();

            SetFoldoutContent(DisplayStyle.None);
        }

        public void Add(ExplorerChildBase child)
        {
            if (child.ChildInfo.Parent is ExplorerFolder folder)
            {
                folder.Remove(child);
            }
            
            child.AssetId.MoveFileTo(AssetId);
            
            AddChild(child);

            SetFoldoutContent(DisplayStyle.Flex);

            SetIcon(_filesFolderIcon);
        }

        private void Remove(ExplorerChildBase child)
        {
            OnDeletedChildHandle(child);
        }

        private void LoadFolder()
        {
            var path = AssetDatabase.GUIDToAssetPath(AssetId);

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            for (int index = 0, count = files.Length; index < count; index++)
            {
                var file = files[index];

                var fileInfo = new FileInfo(file);

                if (fileInfo.Extension == ".meta")
                {
                    continue;
                }

                var assetId = AssetDatabase.GUIDFromAssetPath(file);
                var dataType = fileInfo.ParseDataType();

                CreateFileView(assetId, dataType);
            }

            for (int index = 0, count = directories.Length; index < count; index++)
            {
                var directory = directories[index];

                var assetId = AssetDatabase.GUIDFromAssetPath(directory);

                CreateFolderView(assetId);
            }
        }

        protected override void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Create/Folder", OnCreateNewFolder);
            @event.menu.AppendAction("Create/Dialog", _ => OnCreateNewFile(DataType.DIALOG, FileType.JSON));
            @event.menu.AppendAction("Create/Blackboard", _ => OnCreateNewFile(DataType.BLACKBOARD, FileType.JSON));
            @event.menu.AppendAction("Create/Character", _ => OnCreateNewFile(DataType.CHARACTER, FileType.SCRIPTABLE_OBJECT));
            @event.menu.AppendAction("Create/Chapter", _ => OnCreateNewFile(DataType.CHAPTER, FileType.SCRIPTABLE_OBJECT));
            @event.menu.AppendAction("Create/Location", _ => OnCreateNewFile(DataType.LOCATION, FileType.SCRIPTABLE_OBJECT));
            @event.menu.AppendAction("Create/Item", _ => OnCreateNewFile(DataType.ITEM, FileType.SCRIPTABLE_OBJECT));

            base.ContextMenuBuild(@event);
        }

        protected override void OnOneClickHeaderHandle()
        {
            var displayStyle = _contentDisplayStyle == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;

            SetFoldoutContent(displayStyle);
        }

        protected override void OnDeletedChildHandle(ExplorerChildBase child)
        {
            base.OnDeletedChildHandle(child);

            if (Children.Count == 0)
            {
                SetIcon(_emptyFolderIcon);
            }
        }

        private void SetFoldoutContent(DisplayStyle displayStyle)
        {
            _contentDisplayStyle = displayStyle;
            Content.style.display = new StyleEnum<DisplayStyle>(_contentDisplayStyle);
        }

        private void OnCreateNewFile(DataType dataType, FileType fileType)
        {
            var folderPath = AssetDatabase.GUIDToAssetPath(AssetId);

            var targetName = FileSystemUtilsEditor.CreateUniqueNameForDirectory(folderPath, $"new {dataType.ToString().ToLower()}");

            var info = Explorer.CreateFile(targetName, dataType, fileType, this);

            CreateFileView(info, dataType);
        }

        private void OnCreateNewFolder(DropdownMenuAction action)
        {
            var folderPath = AssetDatabase.GUIDToAssetPath(AssetId);

            var targetName = FileSystemUtilsEditor.CreateUniqueNameForDirectory(folderPath, "new Folder");

            var info = Explorer.CreateFolder(targetName, this);

            CreateFolderView(info);
        }

        private void CreateFileView(GUID assetId, DataType dataType)
        {
            var child = TemplateLoader.Get(TemplateType.ExplorerChildTemplate);

            var childInfo = new ExplorerChildInfo
            {
                Explorer = ChildInfo.Explorer,
                Parent = this,
                Root = child
            };

            ExplorerFile file = dataType switch
            {
                DataType.DIALOG => new DialogExplorerFile(assetId, childInfo),
                DataType.CHARACTER => new CharacterExplorerFile(assetId, childInfo),
                DataType.CHAPTER => new ChapterExplorerFile(assetId, childInfo),
                DataType.LOCATION => new LocationExplorerFile(assetId, childInfo),
                DataType.BLACKBOARD => new BlackboardExplorerFile(assetId, childInfo),
                DataType.ITEM => new ItemExplorerFile(assetId, childInfo),
                
                _ => throw new Exception($"Unknown file type: {dataType}. Please specify the correct file type.")
            };

            AddChild(file);

            SetFoldoutContent(DisplayStyle.Flex);

            SetIcon(_filesFolderIcon);
        }

        private void CreateFolderView(GUID assetId)
        {
            var child = TemplateLoader.Get(TemplateType.ExplorerChildTemplate);

            var childInfo = new ExplorerChildInfo
            {
                Explorer = ChildInfo.Explorer,
                Parent = this,
                Root = child
            };

            var folder = new ExplorerFolder(assetId, childInfo);

            AddChild(folder);

            SetFoldoutContent(DisplayStyle.Flex);

            SetIcon(_filesFolderIcon);
        }
    }
}