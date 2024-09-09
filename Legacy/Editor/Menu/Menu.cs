using System;
using System.Collections.Generic;
using System.Linq;
using History.Tools.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class Menu : IDisposable
    {
        public readonly VisualElement Root;

        private const string LEFT_GROUP_CLASS = "menu-left-group";
        private const string RIGHT_GROUP_CLASS = "menu-right-group";

        private readonly VisualElement _leftGroup;
        private readonly VisualElement _rightGroup;
        
        private Toggle _isAutoSaveToggle;
        private Button _loadSpriteButton;

        public Menu(VisualElement root)
        {
            Root = root;

            _leftGroup = root.Q<VisualElement>(className: LEFT_GROUP_CLASS);
            _rightGroup = root.Q<VisualElement>(className: RIGHT_GROUP_CLASS);

            CreateToolbarMenuSettings();
            CreateAutoSaveToggleToolbar();
            CreateLoadSpriteButton();
        }

        private void CreateAutoSaveToggleToolbar()
        {
            _isAutoSaveToggle = new Toggle
            {
                label = "Auto Save",
                value = HistoryEditorWindow.IsAutoSave,
                labelElement =
                {
                    style =
                    {
                        minWidth = new StyleLength(50)
                    }
                }
            };

            _isAutoSaveToggle.RegisterValueChangedCallback(OnChangeIsAutoSaveToggleHandle);

            _rightGroup.Add(_isAutoSaveToggle); 
        }
        
        private void CreateLoadSpriteButton()
        {
            _loadSpriteButton = new Button
            {
                text = "Load Sprite",
            };

            _loadSpriteButton.clicked += OnLoadSpriteHandle;

            _rightGroup.Add(_loadSpriteButton); 
        }

        private void OnLoadSpriteHandle()
        {
            var path = $"{Application.streamingAssetsPath}/Content/Sprites";
            SpriteUtilsEditor.LoadSpriteFromComputer(path);
        }

        private void CreateToolbarMenuSettings()
        {
            CreateToolbarMenu("File", new Dictionary<string, Action<DropdownMenuAction>>
            {
                { "Save", (a) => { HistoryEditorWindow.SaveCurrent(); } },
                { "Save All", (a) => { HistoryEditorWindow.SaveAll(); } },
            });
        }

        private void CreateToolbarMenu(string name, Dictionary<string, Action<DropdownMenuAction>> actions)
        {
            var toolbarMenu = new ToolbarMenu { text = name };

            for (int index = 0, count = actions.Count; index < count; index++)
            {
                var action = actions.ElementAt(index);

                toolbarMenu.menu.AppendAction(action.Key, action.Value);
            }

            _leftGroup.Add(toolbarMenu);
        }

        private void OnChangeIsAutoSaveToggleHandle(ChangeEvent<bool> @event)
        {
            HistoryEditorWindow.IsAutoSave = @event.newValue;
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}