using History.Editor.HistoryEditor.Dialogue.Nodes.Abstracts;
using History.Editor.UIToolkitExtensions;
using Sandbox.Dialogue;

namespace History.Editor.HistoryEditor.Dialogue.Nodes
{
    public class SentenceDialogNodeEditor : DialogNodeEditorBase
    {
        private readonly SentenceDialogNode _data;

        public SentenceDialogNodeEditor(SentenceDialogNode data, DialogueSceneContext context) 
            : base(data, context)
        {
            _data = data;
            var icon = SpriteProvider.Get(SpriteType.dialog_sentence_icon);
            SetIcon(icon);
            
            
            data.OnUpdateText += UpdateName;

            UpdateName();
        }

        public override void Select()
        {
            base.Select();
            
            HistoryEditor.Properties.OpenProperties(_data);
        }
    }
}