using System.IO;
using History.Editor.UIToolkitExtensions;
using Newtonsoft.Json;
using Sandbox.Scripts.Locations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Locations
{
    public class LocationSceneContext : SceneContextBase
    {
        public sealed override VisualElement Root { get; }

        private const string NAME_TEXT_FIELD_CLASS = "name-text-field";
        private const string SPRITE_FIELD_CLASS = "sprite-object-field";

        private readonly SpriteField _spriteField;
        private readonly TextField _nameTextField;

        private Location _location;
        
        public LocationSceneContext(Location location, GUID assetId) : base(assetId)
        {
            _location = location;
            
            Root = TemplateLoader.Get(TemplateType.LocationSceneContext);

            var style = StyleLoader.Get(StyleType.LocationSceneContextStyles);
            Root.styleSheets.Add(style);

            _spriteField = new SpriteField(Root.Q<VisualElement>(className: SPRITE_FIELD_CLASS));
            _nameTextField = Root.Q<TextField>(className: NAME_TEXT_FIELD_CLASS);

            _nameTextField.RegisterValueChangedCallback(OnChangeNameHandle);
            _spriteField.OnSelect += OnSelectSpriteFileHandle;

            _nameTextField.SetValueWithoutNotify(_location.Name);
        }
        
        public override void Save()
        {
            base.Save();

            var filePath = AssetDatabase.GUIDToAssetPath(AssetId);

            var json = JsonConvert.SerializeObject(_location, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText(filePath, json);
        }

        private void OnSelectSpriteFileHandle(Sprite sprite)
        {
            
        }

        private void OnChangeNameHandle(ChangeEvent<string> evt)
        {
            _location.SetName(evt.newValue);
        }
    }
}