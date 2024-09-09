using History.Editor.UIToolkitExtensions;
using History.Tools;
using History.Tools.Editor;
using Newtonsoft.Json;
using Sandbox.Scripts.Database.Enums;
using Sandbox.Scripts.Items;
using UnityEditor;

namespace History.Editor.HistoryEditor.Items
{
    public class ItemExplorerFile : ExplorerFile
    {
        public override FileType FileType => FileType.SCRIPTABLE_OBJECT;
        public override DataType DataType => DataType.ITEM;

        public ItemExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            var icon = SpriteProvider.Get(SpriteType.item_icon);
            
            SetIcon(icon);
        }

        public override object Read()
        {
            var fileInfo = AssetId.CreateFileInfo();

            var json = fileInfo.ReadToEnd();
            
            return JsonConvert.DeserializeObject<Item>(json);
        }
    }
}