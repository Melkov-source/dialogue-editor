using System;
using System.Reflection;
using History.Editor.UIToolkitExtensions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace History.Editor.HistoryEditor
{
    public class SpriteField : IDisposable
    {
        public readonly VisualElement Root;
        
        public event Action<Sprite> OnSelect;

        private const string SELECT_BUTTON_CLASS = "select-button";
        private const string SPRITE_ICON_CLASS = "sprite-icon";
        
        private readonly Button _selectButton;

        private readonly VisualElement _spriteIcon;

        private readonly ObjectField _objectField;

        private Sprite _sprite;

        public SpriteField(VisualElement root)
        {
            Root = root;

            var style = StyleLoader.Get(StyleType.SpriteFieldStyles);
            root.styleSheets.Add(style);

            _spriteIcon = root.Q<VisualElement>(className: SPRITE_ICON_CLASS);

#pragma warning disable CS0618 // Type or member is obsolete
            _spriteIcon.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit);
#pragma warning restore CS0618 // Type or member is obsolete
            
            _selectButton = root.Q<Button>(className: SELECT_BUTTON_CLASS);

            _selectButton.clicked += OnSelectHandle;

            _objectField = new ObjectField
            {
                objectType = typeof(Sprite),
                style =
                {
                    display = new StyleEnum<DisplayStyle>(DisplayStyle.None)
                }
            };

            _objectField.RegisterValueChangedCallback(OnChangeObjectHandle);
            
            Root.Add(_objectField);
        }

        public void SetValueWithoutNotify(Sprite sprite)
        {
            _spriteIcon.style.backgroundImage = new StyleBackground(sprite);
            _objectField.SetValueWithoutNotify(sprite);
            _sprite = sprite;
        }

        private void OnSelectHandle()
        {
            var methodInfo = _objectField
                .GetType()
                .GetMethod("ShowObjectSelector", BindingFlags.Instance | BindingFlags.NonPublic);

            methodInfo?.Invoke(_objectField, default);
        }

        private void OnChangeObjectHandle(ChangeEvent<Object> @event)
        {
            var sprite = (Sprite)@event.newValue;
            
            SetValueWithoutNotify(sprite);
            OnSelect?.Invoke(sprite);
        }
        
        public void Dispose()
        {
        }
    }
}