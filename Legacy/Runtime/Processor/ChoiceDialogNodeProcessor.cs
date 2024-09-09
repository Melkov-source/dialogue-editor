using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using History.Tools;
using JetBrains.Annotations;
using Sandbox.Scripts.Characters;
using Zenject;

namespace Sandbox.Dialogue
{
    [UsedImplicitly]
    public class ChoiceDialogNodeProcessor : DialogNodeProcessorBase
    {
        public override DialogNodeType Type => DialogNodeType.CHOICE;

        [Inject] private readonly CharacterManager _characterManager;

        public ChoiceDialogNodeProcessor(DialogController controller) : base(controller)
        {
            
        }

        public override async Task<List<DialogNodeBase>> Process(DialogNodeBase node)
        {
            var choiceNode = (ChoiceDialogNode)node;

            var replies = choiceNode.Replies.ToDictionary
            (
                reply => reply.Sentence.GetStableHash(),
                reply => reply
            );

            var repliesTarget = replies.ToDictionary
            (
                reply => reply.Key,
                reply => reply.Value.Sentence
            );
            
            var (speaker, listener) = _characterManager.GetCharacterBySentenceNode(choiceNode);

            Controller.SetChoice
            (
                speaker,
                listener,
                choiceNode.Sentence,
                repliesTarget
            );

            var hash = await Controller.WaitReply();

            var reply = replies[hash];
            
            /*var characterReply = _characterManager.GetCharacterBySentenceNode(choiceNode.CharacterIdSpeaker);
            var sideReply = reply.SideSpeaker;
            
            Controller.SetMessageCharacter(characterReply, sideReply, reply.Sentence);

            await Task.Delay(1000);*/

            return new List<DialogNodeBase> { reply };
        }
    }
}