using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace History.Editor.UIToolkitExtensions
{
    public class SpritesReferenceGenerator
    {
        private const string PATH_ENUM_TEMPLATE_TYPES = "Assets/Project/Scripts/Editor/UIToolkitExtensions/SpriteProvider/Enums/SpriteType.cs";
        
        [MenuItem("Tools/UIToolkit/Generate [Sprites]", priority = 1)]
        public static void RegenerationReferencesUxml()
        {
            var pngFiles = Directory.GetFiles
            (
                "Assets/Project/EditorContent/Sprites", 
                "*.png", 
                SearchOption.AllDirectories
            );
            
            var jpgFiles = Directory.GetFiles
            (
                "Assets/Project/EditorContent/Sprites", 
                "*.jpg", 
                SearchOption.AllDirectories
            );

            var files = new List<string>();
            
            files.AddRange(pngFiles);
            files.AddRange(jpgFiles);
            
            var enumFields = new List<string>();

            for (int index = 0, count = files.Count; index < count; index++)
            {
                var file = files[index];
                var assetPath = file.Replace(Application.dataPath, "").Replace("\\", "/");
                var fileName = Path.GetFileNameWithoutExtension(assetPath);

                enumFields.Add($"\t[SpritePath(\"{assetPath}\")] {fileName},");
            }

            var enumDefinition = new StringBuilder();
            
            enumDefinition.AppendLine("namespace History.Editor.UIToolkitExtensions");
            enumDefinition.AppendLine("{");
            enumDefinition.AppendLine("\tpublic enum SpriteType");
            enumDefinition.AppendLine("\t{");

            foreach (var field in enumFields)
            {
                enumDefinition.AppendLine($"\t{field}");
            }

            enumDefinition.AppendLine("\t}");
            enumDefinition.AppendLine("}");

            File.WriteAllText(PATH_ENUM_TEMPLATE_TYPES, enumDefinition.ToString());

            AssetDatabase.Refresh();

            Debug.Log("Success regeneration references Sprites!");
        }
    }
}