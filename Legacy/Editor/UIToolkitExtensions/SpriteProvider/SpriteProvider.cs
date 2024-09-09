using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public static class SpriteProvider
    {
        private static Dictionary<SpriteType, Sprite> _sprites;

        static SpriteProvider() => LoadTemplates();

        public static Sprite Get(SpriteType spriteType)
        {
            if (_sprites.TryGetValue(spriteType, out var sprite))
            {
                return sprite;
            }

            throw new Exception($"Not found custom sprite: {spriteType} [Sprite]");
        }

        private static void LoadTemplates()
        {
            _sprites = new Dictionary<SpriteType, Sprite>();

            var styleFieldInfos = typeof(SpriteType).GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int index = 0, count = styleFieldInfos.Length; index < count; index++)
            {
                var spriteMeta = styleFieldInfos[index];

                if (spriteMeta.GetValue(null) is not SpriteType type) continue;

                var concreteType = type.GetType().GetField(type.ToString());
                var ussPathAttribute = concreteType.GetCustomAttribute<SpritePathAttribute>();

                var ussPath = ussPathAttribute.Path;

                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ussPath);

                _sprites.Add(type, sprite);
            }
        }
    }
}