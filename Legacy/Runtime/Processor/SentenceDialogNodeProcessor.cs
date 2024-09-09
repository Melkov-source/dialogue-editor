using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sandbox.Scripts.Characters;
using Zenject;

namespace Sandbox.Dialogue
{
    [UsedImplicitly]
    public class SentenceDialogNodeProcessor : DialogNodeProcessorBase
    {
        public override DialogNodeType Type => DialogNodeType.SENTENCE;

        [Inject] private readonly CharacterManager _characterManager;

        public SentenceDialogNodeProcessor(DialogController controller) : base(controller)
        {
        }

        public override async Task<List<DialogNodeBase>> Process(DialogNodeBase node)
        {
            var messageNode = (SentenceDialogNode)node;

            var (speaker, listener) = _characterManager.GetCharacterBySentenceNode(messageNode);

            Controller.SetSentence(speaker, listener, messageNode.Sentence);

            await Controller.WaitNext();

            return new List<DialogNodeBase>();
        }
    }
}