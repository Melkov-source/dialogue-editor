using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace History.Editor.UIToolkitExtensions
{
    public class StylesReferenceGenerator
    {
        private const string PATH_ENUM_TEMPLATE_TYPES = "Assets/Project/Scripts/Editor/UIToolkitExtensions/StyleLoader/Enums/StyleType.cs";
        
        [MenuItem("Tools/UIToolkit/Generate [USS]", priority = 1)]
        public static void RegenerationReferencesUxml()
        {
            var uxmlFiles = Directory.GetFiles
            (
                Application.dataPath, 
                "*.uss", 
                SearchOption.AllDirectories
            );
            
            var enumFields = new List<string>();

            for (int index = 0, count = uxmlFiles.Length; index < count; index++)
            {
                var file = uxmlFiles[index];
                var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace("\\", "/");
                var fileName = Path.GetFileNameWithoutExtension(assetPath);

                enumFields.Add($"\t[USSPath(\"{assetPath}\")] {fileName},");
            }

            var enumDefinition = new StringBuilder();
            
            enumDefinition.AppendLine("namespace History.Editor.UIToolkitExtensions");
            enumDefinition.AppendLine("{");
            enumDefinition.AppendLine("\tpublic enum StyleType");
            enumDefinition.AppendLine("\t{");

            foreach (var field in enumFields)
            {
                enumDefinition.AppendLine($"\t{field}");
            }

            enumDefinition.AppendLine("\t}");
            enumDefinition.AppendLine("}");

            File.WriteAllText(PATH_ENUM_TEMPLATE_TYPES, enumDefinition.ToString());

            AssetDatabase.Refresh();

            Debug.Log("Success regeneration references Styles!");
        }
    }
}