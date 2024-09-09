using History.Editor.UIToolkitExtensions;
using History.Tools;
using History.Tools.Editor;
using Sandbox.Scripts.Database.Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue
{
    public class DialogExplorerFile : ExplorerFile
    {
        public override FileType FileType => FileType.JSON;
        public override DataType DataType => DataType.DIALOG;

        public DialogExplorerFile(GUID assetId, ExplorerChildInfo childInfo) : base(assetId, childInfo)
        {
            var icon = SpriteProvider.Get(SpriteType.dialog_icon);
            
            SetIcon(icon);
        }
        
        public override object Read()
        {
            var fileInfo = AssetId.CreateFileInfo();
            return fileInfo.ReadToEnd();
        }

        protected override void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            base.ContextMenuBuild(@event);
            
            @event.menu.AppendAction("Properties", OnPropertiesHandle);
        }

        private void OnPropertiesHandle(DropdownMenuAction action)
        {
            Debug.Log("Properties");
        }
    }
}