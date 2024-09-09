using System;
using System.IO;
using History.Editor.HistoryEditor.Reflections;
using History.Tools;
using History.Tools.Editor;
using Newtonsoft.Json;
using Sandbox.Scripts.Database;
using Sandbox.Scripts.Database.Enums;
using UnityEditor;
using UnityEngine;

namespace History.Editor.HistoryEditor.Utils
{
    public static class ExplorerUtils
    {
        public static string ParseExtension(this FileType fileType)
        {
            var attribute = fileType.GetAttribute<FileExtensionAttribute>();

            return attribute.Extension;
        }

        public static DataType ParseDataType(this FileInfo fileInfo)
        {
            var fileType = fileInfo.ParseFileType();

            switch (fileType)
            {
                case FileType.SCRIPTABLE_OBJECT:
                    var asset = fileInfo.LoadAsset<ScriptableObjectDataBase>();

                    return asset.DataType;
                case FileType.JSON:
                    var path = fileInfo.FullName;
                    var json = File.ReadAllText(path);

                    var data = JsonConvert.DeserializeObject<DataBase>(json);

                    return data.DataType;
                default:
                    throw new Exception($"Not found data type for target file: {fileInfo.FullName}");
            }
        }

        public static FileType ParseFileType(this FileInfo fileInfo)
        {
            var extension = fileInfo.Extension;

            var type = typeof(FileType);

            var memberInfos = type.GetMembers();

            if (memberInfos.Length <= 0)
            {
                throw new Exception($"Not found member type in enum: {type}");
            }

            var typeAttribute = typeof(FileExtensionAttribute);

            for (int index = 0, count = memberInfos.Length; index < count; index++)
            {
                var memberInfo = memberInfos[index];

                var attributes = memberInfo.GetCustomAttributes(typeAttribute, false);

                var attribute = attributes.Length > 0 ? (FileExtensionAttribute)attributes[0] : default;

                if (attribute == default)
                {
                    continue;
                }

                var fileExtension = attribute.Extension;

                if ($"{fileExtension}" == extension)
                {
                    return (FileType)Enum.Parse(type, memberInfo.Name);
                }
            }

            throw new Exception($"Not found file type for file extension: {extension}");
        }
    }
}