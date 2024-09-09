using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace History.Editor.UIToolkitExtensions
{
    [UsedImplicitly]
    public class TemplateReferenceGenerator
    {
        private const string PATH_ENUM_TEMPLATE_TYPES = "Assets/Project/Scripts/Editor/UIToolkitExtensions/TemplateLoader/Enum/TemplateType.cs";
        
        [MenuItem("Tools/UIToolkit/Generate [UXML]", priority = 1)]
        public static void RegenerationReferencesUxml()
        {
            var uxmlFiles = Directory.GetFiles
            (
                Application.dataPath, 
                "*.uxml", 
                SearchOption.AllDirectories
            );
            
            var enumFields = new List<string>();

            for (int index = 0, count = uxmlFiles.Length; index < count; index++)
            {
                var file = uxmlFiles[index];
                var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace("\\", "/");
                var fileName = Path.GetFileNameWithoutExtension(assetPath);

                enumFields.Add($"\t[UxmlPath(\"{assetPath}\")] {fileName},");
            }

            var enumDefinition = new StringBuilder();
            
            enumDefinition.AppendLine("namespace History.Editor.UIToolkitExtensions");
            enumDefinition.AppendLine("{");
            enumDefinition.AppendLine("\tpublic enum TemplateType");
            enumDefinition.AppendLine("\t{");

            foreach (var field in enumFields)
            {
                enumDefinition.AppendLine($"\t{field}");
            }

            enumDefinition.AppendLine("\t}");
            enumDefinition.AppendLine("}");

            File.WriteAllText(PATH_ENUM_TEMPLATE_TYPES, enumDefinition.ToString());

            AssetDatabase.Refresh();

            Debug.Log("Success regeneration references UXML!");
        }
    }
}