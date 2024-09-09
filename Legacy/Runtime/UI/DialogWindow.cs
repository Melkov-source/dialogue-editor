using System;
using System.Collections.Generic;
using Itibsoft.PanelManager;
using UnityEngine;
using UnityEngine.UI;

namespace Sandbox.Dialogue
{
    public class DialogWindow : PanelBase
    {
        public event Action OnNext;
        public event Action<ushort> OnSelectedReply; 
        
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _nextButton;
        
        [SerializeField] private DialogCharacterView _dialogCharacterSentenceLeft;
        [SerializeField] private DialogCharacterView _dialogCharacterSentenceRight;
        
        [SerializeField] private DialogChoiceBox _dialogChoiceBox;

        public void Initialize()
        {
            _dialogChoiceBox.Initialize();
            _dialogChoiceBox.OnSelected += OnSelectedReplyHandle;
            
            _nextButton.onClick.AddListener(OnNextHandle);
        }
        
        public void SetBackground(Sprite background)
        {
            _backgroundImage.sprite = background;
        }
        
        public void SetSentence(DialogCharacterInfo speaker, DialogCharacterInfo listener, string sentence)
        {
            switch (speaker.Side)
            {
                case DialogSide.LEFT:
                    _dialogCharacterSentenceLeft.gameObject.SetActive(true);
                    _dialogCharacterSentenceRight.gameObject.SetActive(false);
                    
                    _dialogCharacterSentenceLeft.Setup(speaker.Info, listener.Info, sentence);
                    break;
                case DialogSide.RIGHT:
                    _dialogCharacterSentenceRight.gameObject.SetActive(true);
                    _dialogCharacterSentenceLeft.gameObject.SetActive(false);
                    
                    _dialogCharacterSentenceRight.Setup(speaker.Info, listener.Info, sentence);
                    break;
            }
        }
        
        public void SetReplies(Dictionary<ushort, string> replies)
        {
            _dialogChoiceBox.SetReplies(replies);
            
            _dialogChoiceBox.gameObject.SetActive(true);
        }
        
        private void OnNextHandle()
        {
            OnNext?.Invoke();
        }
        
        private void OnSelectedReplyHandle(ushort hash)
        {
            _dialogChoiceBox.gameObject.SetActive(false);
            
            OnSelectedReply?.Invoke(hash);
        }
    }
}