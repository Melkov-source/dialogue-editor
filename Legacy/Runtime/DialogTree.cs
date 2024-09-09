using System.Runtime.Serialization;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class DialogTree
    {
        [DataMember] public RootDialogNode Root = new();
    }
}