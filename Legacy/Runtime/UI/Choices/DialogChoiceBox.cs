using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sandbox.Dialogue
{
    public class DialogChoiceBox : MonoBehaviour
    {
        public event Action<ushort> OnSelected; 
        
        [SerializeField] private Transform _container;

        private DialogChoiceReplyVariant _dialogReplyVariantPrefab;
        
        private readonly List<DialogChoiceReplyVariant> _repliesInstance = new();

        public void Initialize()
        {
            _dialogReplyVariantPrefab = Resources.Load<DialogChoiceReplyVariant>("UI/ReplyVariantItem");
        }

        public void SetReplies(Dictionary<ushort, string> replies)
        {
            if (_repliesInstance.Count > 0)
            {
                foreach (var instance in _repliesInstance)
                {
                    Destroy(instance.gameObject);
                }
            
                _repliesInstance.Clear(); 
            }
            
            foreach (var reply in replies)
            {
                var replyInstance = Instantiate(_dialogReplyVariantPrefab, _container);
                
                replyInstance.Setup(reply);

                replyInstance.OnSelected += OnReplySelectedHandle;
                replyInstance.OnHover += OnReplyHoverHandle;

                _repliesInstance.Add(replyInstance);
            }
        }

        private void OnReplySelectedHandle(DialogChoiceReplyVariant reply)
        {
            var hash = reply.Hash;
            
            OnSelected?.Invoke(hash);
        }
        
        private void OnReplyHoverHandle(DialogChoiceReplyVariant reply)
        {
            for (int index = 0, count = _repliesInstance.Count; index < count; index++)
            {
                var instance = _repliesInstance[index];

                var isHover = instance.Hash.Equals(reply.Hash);
                
                instance.SetActiveSelector(isHover);
            }
        }
    }
}