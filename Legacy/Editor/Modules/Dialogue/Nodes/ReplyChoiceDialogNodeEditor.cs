using History.Editor.HistoryEditor.Dialogue.Nodes.Abstracts;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Nodes
{
    public class ReplyChoiceDialogNodeEditor: DialogNodeEditorBase
    {
        private readonly ReplyChoiceDialogNode _data;
        
        public ReplyChoiceDialogNodeEditor(ReplyChoiceDialogNode data, DialogueSceneContext context) : base(data, context)
        {
            _data = data;
            
            var icon = SpriteProvider.Get(SpriteType.dialog_reply_icon);
            SetIcon(icon);
            
            data.OnUpdateText += UpdateName;

            UpdateName();
            
            for (int index = 0, count = data.Nodes.Count; index < count; index++)
            {
                var node = data.Nodes[index];

                switch (node.Type)
                {
                    case DialogNodeType.SENTENCE:
                    {
                        var nodeView = new SentenceDialogNodeEditor(node as SentenceDialogNode, Context);
                        base.AddNode(nodeView);
                        break; 
                    }
                    case DialogNodeType.CHOICE:
                    {
                        var nodeView = new ChoiceDialogNodeEditor(node as ChoiceDialogNode, Context);
                        base.AddNode(nodeView);
                        break;
                    }
                }
            }
            
           
        }

        protected override void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.AppendAction("Node/Sentence", CreateSentenceNodeHandle);
            @event.menu.AppendAction("Node/Choice", CreateChoiceNodeHandle);
            
            base.ContextMenuBuild(@event);
        }

        public override void Select()
        {
            base.Select();
            
            HistoryEditor.Properties.OpenProperties(_data);
        }

        private void CreateSentenceNodeHandle(DropdownMenuAction action)
        {
            var node = new SentenceDialogNodeEditor(new SentenceDialogNode(), Context);

            AddNode(node);
        }

        private void CreateChoiceNodeHandle(DropdownMenuAction action)
        {
            var node = new ChoiceDialogNode();
            
            var nodeView = new ChoiceDialogNodeEditor(node, Context);

            AddNode(nodeView);
        }

        public override void AddNode(DialogNodeEditorBase node)
        {
            _data.Nodes.Add(node.Data);
            
            base.AddNode(node);
        }

        public override void DeleteNode(DialogNodeEditorBase node)
        {
            _data.Nodes.Remove(node.Data);
            base.DeleteNode(node);
        }
    }
}