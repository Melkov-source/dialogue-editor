using UnityEngine;

namespace Sandbox.Dialogue
{
    [DialogParameterMeta
    (
        Category = "Debug",
        Description = "Unity implement logger.",
        Name = "Debug.Log",
        Type = DialogParameterType.ACTION,
        ConcreteType = DialogActionType.DEBUG_LOG
    )]
    public class DebugLogDialogAction : DialogParameterBase
    {
        public override object Execute(object[] args)
        {
            Debug.Log(args[0]);

            return default;
        }
        
        public class Parameter
        {
            public string Message { get; set; }
        }
    }
}