using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using History.Editor.HistoryEditor.Utils;
using History.Editor.UIToolkitExtensions;
using History.Tools.Editor;
using Newtonsoft.Json;
using Sandbox.Scripts.Database.Enums;
using Sandbox.Scripts.Database.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class Explorer : IDisposable
    {
        public static event Action<GUID> OnOpenFile;
        public static event Action<GUID> OnUpdateFile;
        public static event Action<GUID> OnDeleteFile;

        public static List<ExplorerFolder> Folders => _folders;
        
        private static Explorer _instance;

        private const string EXPLORER_DIRECTORY_ROOT = "Assets/Project/Resources/Database";

        private const string EXPLORER_SCROLL_VIEW_CLASS = "explorer-scroll-view";

        private readonly VisualElement _root;

        private readonly Header _header;
        private readonly ScrollView _scrollView;

        private static List<ExplorerFolder> _folders = new();

        public Explorer(VisualElement root)
        {
            _instance = this;

            _root = root;

            var headerView = root.Q<VisualElement>(className: Header.ROOT_CLASS);
            _scrollView = root.Q<ScrollView>(className: EXPLORER_SCROLL_VIEW_CLASS);

            _header = new Header(headerView);
            
            _header.SetTitle("Explorer");

            _header.OnClose += OnCloseHandle;
            _header.OnPin += OnPinHandle;

            CreateContextMenu();

            CreateExplorer(EXPLORER_DIRECTORY_ROOT);
        }

        public static List<TFileExplorerType> GetFiles<TFileExplorerType>(DataType dataType) 
            where TFileExplorerType : ExplorerFile
        {
            var files = new List<TFileExplorerType>();
            
            for (int indexI = 0, countI = _folders.Count; indexI < countI; indexI++)
            {
                var folder = _folders[indexI];

                var filesInFolder = GetFiles(folder);

                for (int indexJ = 0, countJ = filesInFolder.Count; indexJ < countJ; indexJ++)
                {
                    var file = filesInFolder[indexJ];

                    if (file.DataType == dataType)
                    {
                        files.Add((TFileExplorerType)file);
                    }
                }
            }

            return files;
        }

        public static List<ExplorerChildBase> GetEntities(ExplorerEntityType type)
        {
            var entities = new List<ExplorerChildBase>();
            
            for (int index = 0, count = _folders.Count; index < count; index++)
            {
                var folder = _folders[index];

                if (type == ExplorerEntityType.Folder)
                {
                    entities.Add(folder);
                }
                
                var children = GetChildren(folder);

                foreach (var child in children)
                {
                    if (child.ExplorerType == type)
                    {
                        entities.Add(child);
                    }
                }
            }

            return entities;
        }

        private static void CreateExplorer(string directory)
        {
            _folders = new List<ExplorerFolder>();

            var subdirectories = Directory.GetDirectories(directory);

            for (int index = 0, count = subdirectories.Length; index < count; index++)
            {
                var subdirectory = subdirectories[index];

                var assetId = AssetDatabase.GUIDFromAssetPath(subdirectory);

                CreateFolderView(assetId);
            }
        }

        public static GUID CreateFile(string name, DataType dataType, FileType fileType, ExplorerChildBase parent = default)
        {
            var fileDirectoryPath = GetPathWithParentPath(parent);
            var fileExtension = fileType.ParseExtension();

            var fileName = $"{name}{fileExtension}";
            var filePath = Path.Combine(fileDirectoryPath, fileName);

            var concreteType = dataType.ParseConcreteType();

            switch (fileType)
            {
                case FileType.SCRIPTABLE_OBJECT:
                {
                    var instance = ScriptableObject.CreateInstance(concreteType);

                    return AssetDatabaseUtilsEditor.CreateAsset(filePath, instance);
                }
                case FileType.JSON:
                {
                    var instance = Activator.CreateInstance(concreteType);
                    var content = JsonConvert.SerializeObject(instance, Formatting.Indented);
                    
                    return AssetDatabaseUtilsEditor.CreateFile(filePath, content);
                }
                default:
                    throw new Exception($"Not found file type: {fileType}");
            }
        }

        public static GUID CreateFolder(string nameFolder, ExplorerChildBase parent = default)
        {
            var folderDirectoryPath = GetPathWithParentPath(parent);

            var directoryPath = Path.Combine(folderDirectoryPath, nameFolder);

            var id = AssetDatabaseUtilsEditor.CreateFolder(directoryPath);

            return id;
        }

        private static string GetPathWithParentPath(ExplorerChildBase parent)
        {
            if (parent is not { ExplorerType: ExplorerEntityType.Folder })
            {
                return EXPLORER_DIRECTORY_ROOT;
            }

            var folderPath = AssetDatabase.GUIDToAssetPath(parent.AssetId);

            return folderPath;
        }

        public static void SelectFile(GUID assetId)
        {
            var files = _folders.SelectMany(GetFiles);

            foreach (var file in files)
            {
                if (file.AssetId != assetId)
                {
                    file.UnSelect();
                }
                else
                {
                    file.Select();
                }
            }
        }
        
        public static void UnselectFile(GUID assetId)
        {
            var files = _folders.SelectMany(GetFiles);
            
            foreach (var file in files)
            {
                if (file.AssetId == assetId)
                {
                    file.UnSelect();
                }
            }
        }

        public static void OpenFile(GUID assetId)
        {
            var files = _folders.SelectMany(GetFiles);

            foreach (var file in files)
            {
                if (file.AssetId == assetId)
                {
                    file.Select();
                }
                else
                {
                    file.UnSelect();
                }
            }

            OnOpenFile?.Invoke(assetId);
        }

        public static void UpdateFile(GUID assetId)
        {
            OnUpdateFile?.Invoke(assetId);
        }
        
        public static void DeleteAsset(GUID assetId)
        {
            for (int indexI = 0, countI = _folders.Count; indexI < countI; indexI++)
            {
                var folder = _folders[indexI];

                if (folder.AssetId == assetId)
                {
                    var files = GetFiles(folder);

                    for (int indexJ = 0, countJ = files.Count; indexJ < countJ; indexJ++)
                    {
                        var file = files[indexJ];
                        
                        OnDeleteFile?.Invoke(file.AssetId);
                    }

                    _folders.Remove(folder);
                    AssetDatabaseUtilsEditor.Delete(assetId);
                    return;
                }

                var children = GetChildren(folder);

                for (int indexJ = 0, countJ = children.Count; indexJ < countJ; indexJ++)
                {
                    var asset = children[indexJ];

                    if (asset.AssetId != assetId)
                    {
                        continue;
                    }

                    if (asset.ExplorerType == ExplorerEntityType.Folder)
                    {
                        var files = GetFiles(asset as ExplorerFolder);

                        for (int indexK = 0, countK = files.Count; indexK < countK; indexK++)
                        {
                            var file = files[indexK];
                            OnDeleteFile?.Invoke(file.AssetId);
                        }
                    }
                    else
                    {
                        OnDeleteFile?.Invoke(asset.AssetId);
                    }
                    
                    AssetDatabaseUtilsEditor.Delete(assetId);
                    return;
                }
            }
        }

        private static List<ExplorerFile> GetFiles(ExplorerFolder folder)
        {
            var files = new List<ExplorerFile>();

            for (int index = 0, count = folder.Children.Count; index < count; index++)
            {
                var child = folder.Children.ElementAt(index).Value;

                switch (child)
                {
                    case ExplorerFile fileChild:
                        files.Add(fileChild);
                        break;
                    case ExplorerFolder subFolder:
                    {
                        var filesInSubFolder = GetFiles(subFolder);
                        files.AddRange(filesInSubFolder);
                        break;
                    }
                }
            }

            return files;
        }

        private static List<ExplorerChildBase> GetChildren(ExplorerChildBase folder)
        {
            var children = new List<ExplorerChildBase>();
            
            for (int index = 0, count = folder.Children.Count; index < count; index++)
            {
                var child = folder.Children.ElementAt(index).Value;

                children.Add(child);
                
                switch (child)
                {
                    case ExplorerFolder subFolder:
                    {
                        var subChildren = GetChildren(subFolder);
                        children.AddRange(subChildren);
                        break;
                    }
                }
            }

            return children;
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextualMenuManipulator(ContextMenuBuild);

            _root.AddManipulator(contextMenu);
        }

        private void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Create/Folder", OnCreateNewFolder);
        }

        private void OnCreateNewFolder(DropdownMenuAction action)
        {
            var targetName = FileSystemUtilsEditor.CreateUniqueNameForDirectory(EXPLORER_DIRECTORY_ROOT, "New Folder");

            var info = CreateFolder(targetName);

            CreateFolderView(info);
        }

        private static void CreateFolderView(GUID assetId)
        {
            var child = TemplateLoader.Get(TemplateType.ExplorerChildTemplate);

            var childInfo = new ExplorerChildInfo
            {
                Explorer = _instance,
                Parent = null,
                Root = child
            };

            var folder = new ExplorerFolder(assetId, childInfo);

            _folders.Add(folder);

            _instance._scrollView.Add(child);
        }

        private void OnPinHandle()
        {
            Debug.Log("Explorer.Header.Pin");
        }

        private void OnCloseHandle()
        {
            Debug.Log("Explorer.Header.Close");
        }

        public void Dispose()
        {
            _header?.Dispose();
        }
    }
}