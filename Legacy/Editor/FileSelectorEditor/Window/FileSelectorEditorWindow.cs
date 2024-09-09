using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using History.Editor.FileSelectorEditor.Elements;
using History.Editor.UIToolkitExtensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.FileSelectorEditor
{
    public class FileSelectorEditorWindow : EditorWindow
    {
        private const string CELL_GRID_SCROLL_VIEW_CLASS = "cell-grid-scroll-view";
        private const string SEARCH_FIELD_CLASS = "search-field";
        
        private event Action<FileInfo> _onFileSelect;

        private ScrollView _cellContainer;
        private ToolbarSearchField _toolbarSearchField;

        private List<FileInfo> _fileInfos;
        private Dictionary<FileInfo, FileCellElement> _cells = new();
        
        public static FileSelectorEditorWindow Open()
        {
            var window = GetWindow<FileSelectorEditorWindow>();
            
            window.titleContent = new GUIContent("Asset Selector");

            window.maxSize = new Vector2(881,  600);
            window.minSize = new Vector2(881,  600);
            
            window.Show();

            return window;
        }
        
        private void CreateGUI()
        {
            var view = TemplateLoader.Get(TemplateType.FileSelectorWindow);
            var style = StyleLoader.Get(StyleType.FileSelectorWindowStyles);
            
            rootVisualElement.styleSheets.Add(style);
            rootVisualElement.Add(view);

            _cellContainer = rootVisualElement.Q<ScrollView>(className: CELL_GRID_SCROLL_VIEW_CLASS);
            _toolbarSearchField = rootVisualElement.Q<ToolbarSearchField>(className: SEARCH_FIELD_CLASS);

            _toolbarSearchField.RegisterValueChangedCallback(OnSearchFieldHandle);
        }

        private void OnGUI()
        {
            if (_isInitilized == false)
            {
                return;
            }

            foreach (var cell in _cells.Values)
            {
                cell.Update();
            }
        }

        private void OnSearchFieldHandle(ChangeEvent<string> @event)
        {
            var value = @event.newValue;

            if (value == "")
            {
                ShowOnlyCells(_fileInfos);
                return;
            }

            var filesContains = new List<FileInfo>();

            for (int index = 0, count = _fileInfos.Count; index < count; index++)
            {
                var file = _fileInfos[index];

                if (file.Name.Contains(value))
                {
                    filesContains.Add(file);
                }
            }
            
            ShowOnlyCells(filesContains);
        }

        private bool _isInitilized = false;

        public void Setup(List<FileInfo> files, FileInfo selectedFile, Action<FileInfo> fileSelectAction)
        {
            _isInitilized = false;
            
            for (int index = 0, count = _cells.Count; index < count; index++)
            {
                var pair = _cells.ElementAt(index);
                pair.Value.Root.RemoveFromHierarchy();
            }
            
            _cells.Clear();
            
            _fileInfos = files;
            _onFileSelect = fileSelectAction;

            for (int index = 0, count = files.Count; index < count; index++)
            {
                var file = files[index];

                var fileCellElement = new FileCellElement(file, _cellContainer);
                fileCellElement.OnClick += OnSelectFileHandle;
                _cellContainer.Add(fileCellElement.Root);
                
                _cells.Add(file, fileCellElement);
            }

            SelectFile(selectedFile);
            
            _isInitilized = true;
        }

        private void SelectFile(FileInfo selectedFile)
        {
            for (int index = 0, count = _fileInfos.Count; index < count; index++)
            {
                var file = _fileInfos[index];
                var cell = _cells[file];

                
                if (selectedFile.FullName == file.FullName)
                {
                    cell.Select();
                }
                else
                {
                    cell.Unselect();
                }
            }
        }

        private void ShowOnlyCells(List<FileInfo> files)
        {
            for (int index = 0, count = _fileInfos.Count; index < count; index++)
            {
                var file = _fileInfos[index];

                if (files.Contains(file))
                {
                    _cells[file].Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    continue;
                }
                
                _cells[file].Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        private void OnSelectFileHandle(FileInfo fileInfo)
        {
            SelectFile(fileInfo);
            _onFileSelect?.Invoke(fileInfo);
        }
    }
}