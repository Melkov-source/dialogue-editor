using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sandbox.Dialogue;
using UnityEngine;

namespace Sandbox.Scripts.Characters
{
    [UsedImplicitly]
    public class CharacterManager
    {
        private readonly List<CharacterInfo> _characters = Resources.LoadAll<CharacterInfo>("Database").ToList();
        
        public (DialogCharacterInfo, DialogCharacterInfo) GetCharacterBySentenceNode(SentenceDialogNode sentenceDialogNode)
        {
            var speakerId = sentenceDialogNode.CharacterIdSpeaker;
            var listenerId = sentenceDialogNode.CharacterIdListener;

            var speakerSide = sentenceDialogNode.SideSpeaker;
            var listenerSide = sentenceDialogNode.SideListener;

            CharacterInfo speakerInfo = default;
            CharacterInfo listenerInfo = default;
            
            for (int index = 0, count = _characters.Count; index < count; index++)
            {
                var character = _characters[index];

                if (speakerInfo == default && character.Id.Equals(speakerId))
                {
                    speakerInfo =  character.Copy();
                }

                if (listenerInfo == default && character.Id.Equals(listenerId))
                {
                    listenerInfo = character.Copy();
                }
            }

            if (speakerInfo == default)
            {
                throw new Exception($"Not found speaker character for target id: [{speakerId}]");
            }
            
            if (listenerInfo == default)
            {
                throw new Exception($"Not found listener character for target id: [{listenerId}]");
            }

            var speaker = new DialogCharacterInfo
            {
                Info = speakerInfo,
                Side = speakerSide
            };

            var listener = new DialogCharacterInfo
            {
                Info = listenerInfo,
                Side = listenerSide
            };

            return (speaker, listener);
        }
    }
}