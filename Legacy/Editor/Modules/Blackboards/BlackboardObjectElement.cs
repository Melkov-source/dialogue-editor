using History.Editor.UIToolkitExtensions;
using Sandbox.Scripts.Blackboards;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Blackboards
{
    public class BlackboardObjectElement
    {
        public VisualElement Root;

        private const string KEY_TEXT_FIELD_CLASS = "key-text-field";
        private const string VALUE_TEXT_FIELD_CLASS = "value-text-field";
        
        private DataObject _dataObject;

        private readonly TextField _keyTextField;
        private readonly TextField _valueTextField;

        public BlackboardObjectElement(DataObject dataObject)
        {
            _dataObject = dataObject;

            Root = TemplateLoader.Get(TemplateType.BackboardDataElement);

            _keyTextField = Root.Q<TextField>(className: KEY_TEXT_FIELD_CLASS);
            _valueTextField = Root.Q<TextField>(className: VALUE_TEXT_FIELD_CLASS);

            _keyTextField.RegisterValueChangedCallback(OnKeyChangedHandle);
            _valueTextField.RegisterValueChangedCallback(OnValueChangedHandle);

            _keyTextField.value = dataObject.Key;
            _valueTextField.value = dataObject.Value?.ToString() ?? "";
        }

        private void OnValueChangedHandle(ChangeEvent<string> evt)
        {
            _dataObject.SetValue(evt.newValue);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }

        private void OnKeyChangedHandle(ChangeEvent<string> evt)
        {
            _dataObject.SetKey(evt.newValue);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
    }
}