using System;
using System.IO;
using History.Editor.UIToolkitExtensions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class HistoryEditorWindow : EditorWindow
    {
        public static bool IsAutoSave
        {
            get => _editorData.IsAutoSave;
            set
            {
                _editorData.IsAutoSave = value;
                SaveEditorData();
            }
        }

        public static bool IsShowCharacterDialog
        {
            get => _editorData.IsShowCharacterNameInDialog;
            set
            {
                _editorData.IsShowCharacterNameInDialog = value;
                SaveEditorData();
            }
        }

        public static bool IsShowSideDialog
        {
            get => _editorData.IsShowSideInDialog;
            set
            {
                _editorData.IsShowSideInDialog = value;
                SaveEditorData();
            }
        }

        public static event Action OnSaveCurrent;

        private const string MENU_CLASS = "menu";
        private const string EXPLORER_CLASS = "explorer";
        private const string SCENE_CLASS = "scene";
        private const string PROPERTIES_CLASS = "properties";

        private const string DATA_PATH = "Assets/Project/EditorContent/Data/HistoryEditorData.json";
        private static HistoryEditorData _editorData;

        private Menu _menu;
        private Explorer _explorer;
        private Scene _scene;
        private Properties _properties;


        [MenuItem("Tools/Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<HistoryEditorWindow>();

            window.titleContent = new GUIContent("History Editor");
            window.Show();
        }

        private void CreateGUI()
        {
            var json = File.ReadAllText(DATA_PATH);
            _editorData = JsonConvert.DeserializeObject<HistoryEditorData>(json);

            var view = TemplateLoader.Get(TemplateType.MainWindow);
            var style = StyleLoader.Get(StyleType.MainWindowStyles);

            var menuView = view.Q<VisualElement>(className: MENU_CLASS);
            var explorerView = view.Q<VisualElement>(className: EXPLORER_CLASS);
            var sceneView = view.Q<VisualElement>(className: SCENE_CLASS);
            var propertiesView = view.Q<VisualElement>(className: PROPERTIES_CLASS);

            _explorer = new Explorer(explorerView);
            _menu = new Menu(menuView);
            _scene = new Scene(sceneView);
            _properties = new Properties(propertiesView);

            rootVisualElement.styleSheets.Add(style);
            rootVisualElement.Add(view);
        }

        private static void SaveEditorData()
        {
            var json = JsonConvert.SerializeObject(_editorData, Formatting.Indented);
            File.WriteAllText(DATA_PATH, json);
        }

        private void OnGUI()
        {
            EventUpdate();
        }

        public static void SaveCurrent()
        {
            OnSaveCurrent?.Invoke();
            Debug.Log("HistoryEditor.Save: current file!");
        }

        public static void SaveAll()
        {
        }

        private void EventUpdate()
        {
            var @event = Event.current;

            switch (@event.type)
            {
                case EventType.KeyDown:
                    OnKeyDownHandle(@event);
                    break;
            }
        }

        private void OnKeyDownHandle(Event @event)
        {
            if (@event.keyCode == KeyCode.S && @event.control)
            {
                @event.Use();
                SaveCurrent();
            }
        }
    }
}