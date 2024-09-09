using History.Editor.HistoryEditor.Dialogue.Nodes.Abstracts;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Nodes
{
    public class ChoiceDialogNodeEditor : DialogNodeEditorBase
    {
        private readonly ChoiceDialogNode _data;
        public ChoiceDialogNodeEditor(ChoiceDialogNode data, DialogueSceneContext context) : base(data, context)
        {
            _data = data;
            
            var icon = SpriteProvider.Get(SpriteType.dialog_quest_mark_icon);
            SetIcon(icon);

            data.OnUpdateText += UpdateName;

            UpdateName();
            
            for (int index = 0, count = data.Replies.Count; index < count; index++)
            {
                var reply = data.Replies[index];

                var nodeView = new ReplyChoiceDialogNodeEditor(reply, Context);
                base.AddNode(nodeView);
            }
        }

        public override void Select()
        {
            base.Select();
            
            HistoryEditor.Properties.OpenProperties(_data);
        }

        protected override void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Node/Reply", CreateReplyNodeHandle);
            base.ContextMenuBuild(@event);
        }

        private void CreateReplyNodeHandle(DropdownMenuAction action)
        {
            var node = new ReplyChoiceDialogNode();
            var nodeView = new ReplyChoiceDialogNodeEditor(node, Context);

            AddNode(nodeView);
        }

        public override void AddNode(DialogNodeEditorBase node)
        {
            _data.Replies.Add(node.Data as ReplyChoiceDialogNode);
            base.AddNode(node);
        }

        public override void DeleteNode(DialogNodeEditorBase node)
        {
            _data.Replies.Remove(node.Data as ReplyChoiceDialogNode);
            base.DeleteNode(node);
        }
    }
}