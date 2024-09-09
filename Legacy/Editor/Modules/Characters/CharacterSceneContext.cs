using History.Editor.UIToolkitExtensions;
using Sandbox.Scripts.Characters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CharacterInfo = Sandbox.Scripts.Characters.CharacterInfo;

namespace History.Editor.HistoryEditor.Characters
{
    public class CharacterSceneContext : SceneContextBase
    {
        public sealed override VisualElement Root { get; }

        private const string SPRITE_OBJECT_FIELD_CLASS = "sprite-object-field";
        private const string ID_TEXT_FIELD_CLASS = "id-text-field";
        private const string NAME_TEXT_FIELD_CLASS = "name-text-field";
        private const string IS_PLAYER_TOGGLE_CLASS = "is-player-toggle";
        
        private readonly SpriteField _spriteField;
        private readonly TextField _idTextField;
        private readonly TextField _nameTextField;
        private readonly Toggle _isPlayerToggle;
        
        private readonly CharacterInfo _characterInfo;
        
        public CharacterSceneContext(CharacterInfo characterInfo, GUID assetId) : base(assetId)
        {
            _characterInfo = characterInfo;
            
            Root = TemplateLoader.Get(TemplateType.CharacterSceneContext);

            var style = StyleLoader.Get(StyleType.CharacterSceneContextStyles);
            Root.styleSheets.Add(style);
            
            _spriteField = new SpriteField(Root.Q<VisualElement>(className: SPRITE_OBJECT_FIELD_CLASS));
            _idTextField = Root.Q<TextField>(className: ID_TEXT_FIELD_CLASS);
            _nameTextField = Root.Q<TextField>(className: NAME_TEXT_FIELD_CLASS);
            _isPlayerToggle = Root.Q<Toggle>(className: IS_PLAYER_TOGGLE_CLASS);
            
            _spriteField.OnSelect += OnSelectSpriteHandle;
            _nameTextField.RegisterValueChangedCallback(OnChangeNameHandle);
            _isPlayerToggle.RegisterValueChangedCallback(OnChangeIsPlayerHandle);
            
            
            _spriteField.SetValueWithoutNotify(characterInfo.Picture);
            _idTextField.SetValueWithoutNotify(characterInfo.Id.ToString());
            _nameTextField.SetValueWithoutNotify(characterInfo.Name);
            _isPlayerToggle.SetValueWithoutNotify(characterInfo.IsPlayer);
        }
        
        public override void Save()
        {
            base.Save();
            
            AssetDatabase.SaveAssets();
        }

        private void OnSelectSpriteHandle(Sprite sprite)
        {
            _characterInfo.SetPicture(sprite);
            
            EditorUtility.SetDirty(_characterInfo);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }

        private void OnChangeNameHandle(ChangeEvent<string> @event)
        {
            _characterInfo.SetName(@event.newValue);
            
            EditorUtility.SetDirty(_characterInfo);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }

        private void OnChangeIsPlayerHandle(ChangeEvent<bool> @event)
        {
            _characterInfo.SetIsPlayer(@event.newValue);
            
            EditorUtility.SetDirty(_characterInfo);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
    }
}