using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sandbox.Dialogue
{
    [DataContract]
    public abstract class DialogNodeBase
    {
        [DataMember] public abstract DialogNodeType Type { get; }
        
        [DataMember] public Guid CharacterIdSpeaker;
        [DataMember] public Guid CharacterIdListener;
        
        [DataMember] public DialogSide SideSpeaker;
        [DataMember] public DialogSide SideListener;
        
        [DataMember] public List<DialogParameterInfo> Conditions = new();
        [DataMember] public List<DialogParameterInfo> Actions = new();
        
        public event Action OnUpdateSideSpeaker;
        public event Action OnUpdateCharacterSpeaker;

        public void SetCharacterSpeaker(Guid characterId)
        {
            CharacterIdSpeaker = characterId;
            OnUpdateCharacterSpeaker?.Invoke();
        }

        public void SetSideSpeaker(DialogSide side)
        {
            SideSpeaker = side;
            OnUpdateSideSpeaker?.Invoke();
        }
        
        public void SetCharacterListener(Guid characterId)
        {
            CharacterIdListener = characterId;
            OnUpdateCharacterSpeaker?.Invoke();
        }

        public void SetSideListener(DialogSide side)
        {
            SideListener = side;
            OnUpdateSideSpeaker?.Invoke();
        }
    }
}