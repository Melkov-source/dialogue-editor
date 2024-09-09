using Sandbox.Scripts.Database.Enums;
using UnityEditor;

namespace History.Editor.HistoryEditor
{
    public abstract class ExplorerFile : ExplorerChildBase
    {
        public abstract FileType FileType { get; }
        public abstract DataType DataType { get; }
        public override int Order => 1;
        public override ExplorerEntityType ExplorerType => ExplorerEntityType.File;

        protected ExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            DragAndDropManipulator manipulator = new(this);
        }
        
        public abstract object Read();

        protected override void OnOneClickHeaderHandle()
        {
            Explorer.OpenFile(AssetId);
        }
    }
}