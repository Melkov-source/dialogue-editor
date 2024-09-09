using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = Sandbox.Scripts.Characters.CharacterInfo;

namespace Sandbox.Dialogue
{
    public class DialogCharacterView : MonoBehaviour
    {
        [SerializeField] private Image _characterSpeakerImage;
        [SerializeField] private Image _characterListenerImage;

        [SerializeField] private DialogMessageBox _dialogMessageBox;

        public void Setup(CharacterInfo speaker, CharacterInfo listener, string sentence)
        {
            _characterSpeakerImage.sprite = speaker.Picture;
            _characterListenerImage.sprite = listener.Picture;
            
            _dialogMessageBox.Setup(speaker.Name, sentence);
        }
    }
}