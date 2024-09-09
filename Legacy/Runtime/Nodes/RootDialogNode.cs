using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class RootDialogNode : DialogNodeBase
    {
        [DataMember] public override DialogNodeType Type => DialogNodeType.ROOT;
        [DataMember] public List<DialogNodeBase> Nodes = new();
    }
}