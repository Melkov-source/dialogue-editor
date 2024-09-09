using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public static class StyleLoader
    {
        private static Dictionary<StyleType, EditorStyle> _styles;

        static StyleLoader() => LoadTemplates();

        public static StyleSheet Get(StyleType styleType)
        {
            if (_styles.TryGetValue(styleType, out var style))
            {
                return style.StyleSheet;
            }

            throw new Exception($"Not found custom style: {styleType} [uss]");
        }

        private static void LoadTemplates()
        {
            _styles = new Dictionary<StyleType, EditorStyle>();

            var styleFieldInfos = typeof(StyleType).GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int index = 0, count = styleFieldInfos.Length; index < count; index++)
            {
                var styleMeta = styleFieldInfos[index];

                if (styleMeta.GetValue(null) is not StyleType type) continue;

                var concreteType = type.GetType().GetField(type.ToString());
                var ussPathAttribute = concreteType.GetCustomAttribute<USSPathAttribute>();

                var ussPath = ussPathAttribute.Path;

                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
                var editorStyle = new EditorStyle(styleSheet);

                _styles.Add(type, editorStyle);
            }
        }
    }
}