using System;
using System.Runtime.Serialization;

namespace Sandbox.Dialogue
{
    [DataContract]
    public class SentenceDialogNode : DialogNodeBase
    {
        [DataMember] public override DialogNodeType Type => DialogNodeType.SENTENCE;
        
        [DataMember] public string Sentence = "";
        
        public event Action OnUpdateText;
        
        public void SetText(string text)
        {
            Sentence = text;
            OnUpdateText?.Invoke();
        }
    }
}