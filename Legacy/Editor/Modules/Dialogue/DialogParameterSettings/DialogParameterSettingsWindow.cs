using System;
using System.Collections.Generic;
using History.Editor.HistoryEditor.Dialogue.Abstracts;
using History.Editor.HistoryEditor.Dialogue.Settings;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue
{
    public class DialogParameterSettingsWindow : EditorWindow
    {
        private const string BUTTON_OK_CLASS = "button-ok";
        private const string BUTTON_BACK_CLASS = "button-back";

        private const string CONTENT_CLASS = "content";

        private const string TITTLE_LABEL_CLASS = "tittle-label";

        private DialogParameterSettingsEditor _parameterSettingsEditor;
        
        private Label _tittleLabel;
        
        private VisualElement _content;
        
        private Button _okButton;
        private Button _backButton;

        private DialogParameterInfo _parameterInfo;
        private Action<DialogParameterInfo> _onBackAction;
        
        public static DialogParameterSettingsWindow Open()
        {
            var window = GetWindow<DialogParameterSettingsWindow>();

            window.titleContent = new GUIContent("Dialog Action Settings");
            
            window.minSize = new Vector2(500,  350);
            window.maxSize = new Vector2(500,  350);
            
            window.Show();

            return window;
        }
        
        private void CreateGUI()
        {
            var view = TemplateLoader.Get(TemplateType.DialogParameterSettings);
            var style = StyleLoader.Get(StyleType.DialogParameterSettingsStyles);

            _tittleLabel = view.Q<Label>(className: TITTLE_LABEL_CLASS);

            _content = view.Q<VisualElement>(className: CONTENT_CLASS);

            _okButton = view.Q<Button>(className: BUTTON_OK_CLASS);
            _backButton = view.Q<Button>(className: BUTTON_BACK_CLASS);

            _okButton.clicked += OnOkayHandle;
            _backButton.clicked += OnBackHandle;

            rootVisualElement.styleSheets.Add(style);
            rootVisualElement.Add(view);
        }

        private void OnBackHandle()
        {
            _onBackAction?.Invoke(_parameterInfo);
            Close();
        }

        private void OnOkayHandle()
        {
            Close();
        }

        public void Setup(DialogParameterInfo info, Action<DialogParameterInfo> onBackAction)
        {
            _parameterInfo = info;
            _onBackAction = onBackAction;

            _tittleLabel.text = "Parameters of " + info.Type;

            switch (info.ParameterType)
            {
                case DialogParameterType.ACTION:
                    var actionType = Enum.Parse<DialogActionType>(info.Type.ToString());
                    SetupActionParameter(info, actionType);
                    break;
                case DialogParameterType.CONDITION:
                    break;
            }
        }

        private void SetupActionParameter(DialogParameterInfo info, DialogActionType actionType)
        {
            switch (actionType)
            {
                case DialogActionType.DEBUG_LOG:
                    _parameterSettingsEditor = new DebugLogParameterSettings(info);
                    break;
            }
            
            _content.Add(_parameterSettingsEditor.Root);
        }
        
        private void SetupConditionParameter(DialogParameterInfo info, DialogConditionType conditionType)
        {
            switch (conditionType)
            {
                case DialogConditionType.BLACKBOARD_VALUE:
                  
                    break;
            }
            
            _content.Add(_parameterSettingsEditor.Root);
        }
    }
}