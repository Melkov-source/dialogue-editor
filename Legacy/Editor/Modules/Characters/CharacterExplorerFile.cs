using History.Editor.UIToolkitExtensions;
using History.Tools.Editor;
using Sandbox.Scripts.Characters;
using Sandbox.Scripts.Database.Enums;
using UnityEditor;

namespace History.Editor.HistoryEditor.Characters
{
    public class CharacterExplorerFile : ExplorerFile
    {
        public override FileType FileType => FileType.SCRIPTABLE_OBJECT;
        public override DataType DataType => DataType.CHARACTER;

        public CharacterExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            var icon = SpriteProvider.Get(SpriteType.character_icon);
            
            SetIcon(icon);
        }
        
        public override object Read()
        {
            return AssetId.LoadAsset<CharacterInfo>();
        }
    }
}