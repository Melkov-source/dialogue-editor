using System.Runtime.Serialization;
using Sandbox.Scripts.Database;
using Sandbox.Scripts.Database.Enums;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class Dialog : DataBase
    {
         [DataMember] public override DataType DataType => DataType.DIALOG;
        
         [DataMember] public DialogTree Tree = new();
    }
}