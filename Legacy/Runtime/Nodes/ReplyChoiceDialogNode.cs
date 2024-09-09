using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class ReplyChoiceDialogNode : SentenceDialogNode
    {
        [DataMember] public override DialogNodeType Type => DialogNodeType.CHOICE_REPLY;
        [DataMember] public List<DialogNodeBase> Nodes = new();
    }
}