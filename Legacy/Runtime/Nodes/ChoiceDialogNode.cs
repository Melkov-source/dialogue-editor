using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class ChoiceDialogNode : SentenceDialogNode
    {
        [DataMember] public override DialogNodeType Type => DialogNodeType.CHOICE;
        
        [DataMember] public List<ReplyChoiceDialogNode> Replies = new();
    }
}