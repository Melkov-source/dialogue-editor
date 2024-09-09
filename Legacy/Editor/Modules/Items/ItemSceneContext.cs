using History.Editor.UIToolkitExtensions;
using Sandbox.Scripts.Items;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Items
{
    public class ItemSceneContext : SceneContextBase
    {
        public sealed override VisualElement Root { get; }

        private const string SPRITE_OBJECT_FIELD_CLASS = "sprite-object-field";
        private const string ID_TEXT_FIELD_CLASS = "id-text-field";
        private const string NAME_TEXT_FIELD_CLASS = "name-text-field";
        private const string DESCRIPTION_TEXT_FIELD_CLASS = "description-text-field";
        
        private readonly SpriteField _spriteField;
        private readonly TextField _idTextField;
        private readonly TextField _nameTextField;
        private readonly TextField _descriptionTextField;
        
        private readonly Item _item;
        
        public ItemSceneContext(Item item, GUID assetId) : base(assetId)
        {
            _item = item;
            
            Root = TemplateLoader.Get(TemplateType.ItemSceneContext);

            var style = StyleLoader.Get(StyleType.ItemrSceneContextStyles);
            Root.styleSheets.Add(style);
            
            _spriteField = new SpriteField(Root.Q<VisualElement>(className: SPRITE_OBJECT_FIELD_CLASS));
            //_idTextField = Root.Q<TextField>(className: ID_TEXT_FIELD_CLASS);
            _nameTextField = Root.Q<TextField>(className: NAME_TEXT_FIELD_CLASS);
            _descriptionTextField = Root.Q<TextField>(className: DESCRIPTION_TEXT_FIELD_CLASS);
            
            _spriteField.OnSelect += OnSelectSpriteHandle;
            _nameTextField.RegisterValueChangedCallback(OnChangeNameHandle);
            _descriptionTextField.RegisterValueChangedCallback(OnChangeDescriptionHandle);
            
            _spriteField.SetValueWithoutNotify(item.Icon);
            //_idTextField.SetValueWithoutNotify(item.Id.ToString());
            _nameTextField.SetValueWithoutNotify(item.Name);
            _descriptionTextField.SetValueWithoutNotify(item.Description);
        }
        
        private void OnSelectSpriteHandle(Sprite sprite)
        {
            _item.Icon = sprite;
            
            EditorUtility.SetDirty(_item);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }

        private void OnChangeNameHandle(ChangeEvent<string> @event)
        {
            _item.Name = @event.newValue;
            
            EditorUtility.SetDirty(_item);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
        
        private void OnChangeDescriptionHandle(ChangeEvent<string> @event)
        {
            _item.Description = @event.newValue;
            
            EditorUtility.SetDirty(_item);
            
            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
    }
}