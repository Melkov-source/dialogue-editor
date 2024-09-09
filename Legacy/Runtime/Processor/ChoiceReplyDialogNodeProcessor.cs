using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sandbox.Scripts.Characters;
using Zenject;

namespace Sandbox.Dialogue
{
    [UsedImplicitly]
    public class ChoiceReplyDialogNodeProcessor : DialogNodeProcessorBase
    {
        public override DialogNodeType Type => DialogNodeType.CHOICE_REPLY;
        
        [Inject] private readonly CharacterManager _characterManager;
        
        public ChoiceReplyDialogNodeProcessor(DialogController controller) : base(controller)
        {
            
        }

        public override async Task<List<DialogNodeBase>> Process(DialogNodeBase node)
        {
            var replyNode = (ReplyChoiceDialogNode)node;
            
            var (speaker, listener) = _characterManager.GetCharacterBySentenceNode(replyNode);

            Controller.SetSentence(speaker, listener, replyNode.Sentence);

            await Task.Delay(1000);

            return replyNode.Nodes;
        }
    }
}