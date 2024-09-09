using History.Editor.UIToolkitExtensions;
using History.Tools;
using History.Tools.Editor;
using Newtonsoft.Json;
using Sandbox.Scripts.Database.Enums;
using Sandbox.Scripts.Locations;
using UnityEditor;

namespace History.Editor.HistoryEditor.Locations
{
    public class LocationExplorerFile : ExplorerFile
    {
        public override FileType FileType => FileType.SCRIPTABLE_OBJECT;
        public override DataType DataType => DataType.LOCATION;

        public LocationExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            var icon = SpriteProvider.Get(SpriteType.location_icon);
            
            SetIcon(icon);
        }
        
        public override object Read()
        {
            var fileInfo = AssetId.CreateFileInfo();

            var json = fileInfo.ReadToEnd();
            
            return JsonConvert.DeserializeObject<Location>(json);
        }
    }
}