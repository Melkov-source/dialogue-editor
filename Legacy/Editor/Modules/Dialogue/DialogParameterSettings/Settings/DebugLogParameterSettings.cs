using System.Collections.Generic;
using History.Editor.HistoryEditor.Dialogue.Abstracts;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Settings
{
    public class DebugLogParameterSettings : DialogParameterSettingsEditor
    {
        private const string MESSAGE_TEXT_FIELD_CLASS = "message-text-field";

        private readonly DialogParameterInfo _actionInfo;
        
        public DebugLogParameterSettings(DialogParameterInfo actionInfo)
        {
            _actionInfo = actionInfo;
            
            Root = TemplateLoader.Get(TemplateType.DebugLogActionParameterSettings);

            var messageTextField = Root.Q<TextField>(className: MESSAGE_TEXT_FIELD_CLASS);

            if (actionInfo.Arguments?.Count > 0)
            {
                messageTextField.SetValueWithoutNotify(actionInfo.Arguments[0].ToString());
            }

            messageTextField.RegisterValueChangedCallback(OnMessageChangeHandle);
        }

        private void OnMessageChangeHandle(ChangeEvent<string> @event)
        {
            _actionInfo.Arguments = new List<object>
            {
                @event.newValue
            };
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
    }
}