using History.Editor.UIToolkitExtensions;
using History.Tools;
using History.Tools.Editor;
using Newtonsoft.Json;
using Sandbox.Scripts.Chapters;
using Sandbox.Scripts.Database.Enums;
using UnityEditor;

namespace History.Editor.HistoryEditor.Chapters
{
    public class ChapterExplorerFile : ExplorerFile
    {
        public override FileType FileType => FileType.SCRIPTABLE_OBJECT;
        public override DataType DataType => DataType.CHAPTER;

        public ChapterExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            var icon = SpriteProvider.Get(SpriteType.chapter_icon);

            SetIcon(icon);
        }

        public override object Read()
        {
            var fileInfo = AssetId.CreateFileInfo();

            var json = fileInfo.ReadToEnd();

            return JsonConvert.DeserializeObject<Chapter>(json);
        }
    }
}