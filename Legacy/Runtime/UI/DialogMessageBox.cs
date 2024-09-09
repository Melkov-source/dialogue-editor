using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sandbox.Dialogue
{
    public class DialogMessageBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sentenceText;
        [SerializeField] private TMP_Text _characterNameText;
        public void Setup(string characterName, string sentence)
        {
            //_characterNameText.text = characterName;
            //_sentenceText.text = sentence;

            //var parent = (RectTransform)_characterNameText.rectTransform.parent;
            //
            //LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
        }
    }
}