using History.Editor.HistoryEditor.Dialogue.Nodes.Abstracts;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Nodes
{
    public sealed class RootDialogNodeEditor : DialogNodeEditorBase
    {
        private readonly RootDialogNode _data;
        
        public RootDialogNodeEditor(RootDialogNode data, DialogueSceneContext context) : base(data, context)
        {
            _data = data;
            
            var icon = SpriteProvider.Get(SpriteType.root_point_icon);
            SetIcon(icon);
            
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
            
            SetFoldoutContent(DisplayStyle.Flex);
        }

        public override void UpdateName()
        {
            SetName("<color=#F2C14E>Root</color>");
        }

        public override void Select()
        {
            base.Select();
            
            HistoryEditor.Properties.CloseProperties();
        }

        protected override void ContextMenuBuild(ContextualMenuPopulateEvent @event)
        {
            @event.menu.ClearItems();
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