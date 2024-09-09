using System;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Properties
{
    public class DialogParameterElement
    {
        public readonly VisualElement Root;

        public event Action<DialogParameterInfo> OnEdit;
        public event Action<DialogParameterInfo> OnDelete;

        private const string ICON_CLASS = "icon";
        private const string NAME_CLASS = "name";
        private const string EDIT_BUTTON_CLASS = "edit-button";
        private const string DELETE_BUTTON_CLASS = "delete-button";

        private readonly Label _nameLabel;
        private readonly VisualElement _iconElement;

        private readonly Button _editButton;
        private readonly Button _deleteButton;

        private readonly DialogParameterInfo _parameter;

        public DialogParameterElement(DialogParameterInfo parameter)
        {
            _parameter = parameter;

            Root = TemplateLoader.Get(TemplateType.DialogParameterElement);

            _iconElement = Root.Q<VisualElement>(className: ICON_CLASS);
            _nameLabel = Root.Q<Label>(className: NAME_CLASS);
            _editButton = Root.Q<Button>(className: EDIT_BUTTON_CLASS);
            _deleteButton = Root.Q<Button>(className: DELETE_BUTTON_CLASS);

            _editButton.clicked += OnEditHandle;
            _deleteButton.clicked += OnDeleteHandle;

            var editIcon = SpriteProvider.Get(SpriteType.edit_icon);
            var deleteIcon = SpriteProvider.Get(SpriteType.delete_icon);

            _editButton.style.backgroundImage = new StyleBackground(editIcon);
            _deleteButton.style.backgroundImage = new StyleBackground(deleteIcon);

            switch (parameter.ParameterType)
            {
                case DialogParameterType.ACTION:
                    var actionType = Enum.Parse<DialogActionType>(parameter.Type.ToString());
                    
                    SetName(actionType.ToString());
                    break;
                case DialogParameterType.CONDITION:
                    var conditionType =  Enum.Parse<DialogConditionType>(parameter.Type.ToString());
                    
                    SetName(conditionType.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetName(string name)
        {
            _nameLabel.text = name;
        }

        public void SetIcon(Sprite icon)
        {
            _iconElement.style.backgroundImage = new StyleBackground(icon);
        }

        private void OnEditHandle()
        {
            OnEdit?.Invoke(_parameter);
        }

        private void OnDeleteHandle()
        {
            OnDelete?.Invoke(_parameter);
        }
    }
}