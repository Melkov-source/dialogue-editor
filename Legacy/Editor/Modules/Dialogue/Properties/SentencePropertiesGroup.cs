using System.Collections.Generic;
using System.Linq;
using History.Editor.HistoryEditor.Abstracts;
using History.Editor.HistoryEditor.Characters;
using History.Editor.UIToolkitExtensions;
using History.Tools;
using Sandbox.Dialogue;
using Sandbox.Scripts.Characters;
using Sandbox.Scripts.Database.Enums;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Properties
{
    public class SentencePropertiesGroup : PropertiesGroupBase
    {
        private const string CHARACTER_SPEAKER_DROPDOWN_CLASS = "sentence-character-speaker-dropdown";
        private const string SIDE_SPEAKER_CLASS = "sentence-side-speaker-dropdown";
        
        private const string CHARACTER_LISTENER_DROPDOWN_CLASS = "sentence-character-listener-dropdown";
        private const string SIDE_LISTENER_CLASS = "sentence-side-listener-dropdown";
        
        
        private const string SENTENCE_TEXT_CLASS = "sentence-text-field";
        
        private readonly SentenceDialogNode _sentenceDialogNode;
        
        private readonly DropdownField _characterSpeakerDropdown;
        private readonly DropdownField _characterListenerDropdown;
        
        private readonly DropdownField _sideSpeakerDropdown;
        private readonly DropdownField _sideListenerDropdown;
        
        private readonly TextField _sentenceText;
        
        private readonly DialogParameterContainer _actionsContainer;
        private readonly DialogParameterContainer _conditionsContainer;

        private Dictionary<string, CharacterInfo> _characters = new();
        
        public SentencePropertiesGroup(SentenceDialogNode sentenceDialogNode) : base(TemplateType.SentenceDialogNodeProperties)
        {
            _sentenceDialogNode = sentenceDialogNode;

            _characterSpeakerDropdown = Root.Q<DropdownField>(className: CHARACTER_SPEAKER_DROPDOWN_CLASS);
            _sideSpeakerDropdown = Root.Q<DropdownField>(className: SIDE_SPEAKER_CLASS);
            
            _characterListenerDropdown = Root.Q<DropdownField>(className: CHARACTER_LISTENER_DROPDOWN_CLASS);
            _sideListenerDropdown = Root.Q<DropdownField>(className: SIDE_LISTENER_CLASS);
            
            _sentenceText = Root.Q<TextField>(className: SENTENCE_TEXT_CLASS);
            
            _actionsContainer = new DialogParameterContainer
            (
                ref _sentenceDialogNode.Actions,
                DialogParameterType.ACTION,
                Root.Q<VisualElement>(className: "actions-container")
            );

            _conditionsContainer = new DialogParameterContainer
            (
                ref _sentenceDialogNode.Conditions,
                DialogParameterType.CONDITION,
                Root.Q<VisualElement>(className: "conditions-container")
            );
            
            _sentenceText.SetValueWithoutNotify(sentenceDialogNode.Sentence);

            var sides = ReflectionUtils
                .GetValuesByEnum<DialogSide>(default)
                .Select(value => value.ToString())
                .ToList();

            _sideSpeakerDropdown.choices = sides;
            _sideSpeakerDropdown.SetValueWithoutNotify(sentenceDialogNode.SideSpeaker.ToString());
            _sideSpeakerDropdown.RegisterValueChangedCallback(UpdateSideSpeakerHandle);
            _characterSpeakerDropdown.RegisterValueChangedCallback(UpdateCharacterSpeakerHandle);
            
            _sideListenerDropdown.choices = sides;
            _sideListenerDropdown.SetValueWithoutNotify(sentenceDialogNode.SideListener.ToString());
            _sideListenerDropdown.RegisterValueChangedCallback(UpdateSideListenerHandle);
            _characterListenerDropdown.RegisterValueChangedCallback(UpdateCharacterListenerHandle);
            
            _sentenceText.RegisterValueChangedCallback(UpdateSentenceTextHandle);
        }

        public override void Open()
        {
            _characters = new Dictionary<string, CharacterInfo>();
            
            var files = Explorer.GetFiles<CharacterExplorerFile>(DataType.CHARACTER);

            CharacterInfo selectedCharacterInfoSpeaker = null;
            CharacterInfo selectedCharacterInfoListener = null;
            
            for (int index = 0, count = files.Count; index < count; index++)
            {
                var file = files[index];
                var character = (CharacterInfo)file.Read();

                if (character.Id == _sentenceDialogNode.CharacterIdSpeaker)
                {
                    selectedCharacterInfoSpeaker = character;
                }
                
                if (character.Id == _sentenceDialogNode.CharacterIdListener)
                {
                    selectedCharacterInfoListener = character;
                }
                
                _characters.Add(character.Name, character);
            }

            var choices = _characters
                .Select(c => c.Key)
                .ToList();
            
            _characterSpeakerDropdown.choices = choices;
            _characterListenerDropdown.choices = choices;

            if (selectedCharacterInfoSpeaker != null)
            {
                _characterSpeakerDropdown.SetValueWithoutNotify(selectedCharacterInfoSpeaker.Name);
            }
            
            if (selectedCharacterInfoListener != null)
            {
                _characterListenerDropdown.SetValueWithoutNotify(selectedCharacterInfoListener.Name);
            }
            
            base.Open();
        }
        
        private void UpdateCharacterSpeakerHandle(ChangeEvent<string> @event)
        {
            UpdateCharacterHandle(@event, "speaker");
        }
        
        private void UpdateCharacterListenerHandle(ChangeEvent<string> @event)
        {
            UpdateCharacterHandle(@event, "listener");
        }
        
        private void UpdateSideSpeakerHandle(ChangeEvent<string> @event)
        {
            UpdateSideHandle(@event, "speaker");
        }
        
        private void UpdateSideListenerHandle(ChangeEvent<string> @event)
        {
            UpdateSideHandle(@event, "listener");
        }
        

        private void UpdateCharacterHandle(ChangeEvent<string> @event, string type)
        {
            var character = _characters[@event.newValue];

            switch (type)
            {
                case "speaker":
                    _sentenceDialogNode.SetCharacterSpeaker(character.Id);
                    break;
                case "listener":
                    _sentenceDialogNode.SetCharacterListener(character.Id);
                    break;
            }

            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
        
        private void UpdateSideHandle(ChangeEvent<string> evt, string type)
        {
            var value = evt.newValue;
            var side = value.ParseEnum<DialogSide>();
            
            switch (type)
            {
                case "speaker":
                    _sentenceDialogNode.SetSideSpeaker(side);
                    break;
                case "listener":
                    _sentenceDialogNode.SetSideListener(side);
                    break;
            }
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }

        private void UpdateSentenceTextHandle(ChangeEvent<string> @event)
        {
            _sentenceDialogNode.SetText(@event.newValue);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
    }
}