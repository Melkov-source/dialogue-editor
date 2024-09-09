using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sandbox.Dialogue
{
    public class DialogChoiceReplyVariant : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public ushort Hash { get; private set; }
        
        public event Action<DialogChoiceReplyVariant> OnSelected;
        public event Action<DialogChoiceReplyVariant> OnHover;

        [SerializeField] private GameObject _selector;
        [SerializeField] private TMP_Text _messageText;

        public void Setup(KeyValuePair<ushort, string> info)
        {
            Hash = info.Key;
            SetText(info.Value);
            
            SetActiveSelector(false);
        }
        
        public void SetActiveSelector(bool enable) => _selector.SetActive(enable);
        
        private void SetText(string message) => _messageText.text = message;
        

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnHover?.Invoke(this);

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => OnSelected?.Invoke(this);
    }
}