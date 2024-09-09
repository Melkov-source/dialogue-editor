using History.Editor.UIToolkitExtensions;
using History.Tools;
using History.Tools.Editor;
using Newtonsoft.Json;
using Sandbox.Scripts.Blackboards;
using Sandbox.Scripts.Database.Enums;
using UnityEditor;

namespace History.Editor.HistoryEditor.Blackboards
{
    public class BlackboardExplorerFile : ExplorerFile
    {
        public override FileType FileType => FileType.JSON;
        public override DataType DataType => DataType.BLACKBOARD;

        public BlackboardExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            var icon = SpriteProvider.Get(SpriteType.blackboard_icon);
            
            SetIcon(icon);
        }
        
        public override object Read()
        {
            var fileInfo = AssetId.CreateFileInfo();

            var json = fileInfo.ReadToEnd();
            
            return JsonConvert.DeserializeObject<Blackboard>(json);
        }
    }
}