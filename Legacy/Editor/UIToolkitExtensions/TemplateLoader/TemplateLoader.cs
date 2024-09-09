using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public static class TemplateLoader
    {
        private static Dictionary<TemplateType, EditorTemplate> _templates;

        static TemplateLoader() => LoadTemplates();

        public static VisualElement Get(TemplateType templateType)
        {
            if (_templates.TryGetValue(templateType, out var template))
            {
                return template.Create();
            }

            throw new Exception($"Not found custom template: {templateType} [uxml]");
        }

        private static void LoadTemplates()
        {
            _templates = new Dictionary<TemplateType, EditorTemplate>();

            var templateMetas = typeof(TemplateType).GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int index = 0, count = templateMetas.Length; index < count; index++)
            {
                var templateMeta = templateMetas[index];

                if (templateMeta.GetValue(null) is not TemplateType type) continue;

                var concreteType = type.GetType().GetField(type.ToString());
                var uxmlPathAttribute = concreteType.GetCustomAttribute<UxmlPathAttribute>();

                var uxmlPath = uxmlPathAttribute.Path;

                var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
                var editorTemplate = new EditorTemplate(visualTreeAsset);

                _templates.Add(type, editorTemplate);
            }
        }
    }
}