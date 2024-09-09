using System;
using System.Runtime.Serialization;
using History.Tools;
using Sandbox.Scripts.Database;
using Sandbox.Scripts.Database.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sandbox.Scripts.Characters
{
    public class CharacterInfo : ScriptableObjectDataBase
    {
        public override DataType DataType => DataType.CHARACTER;
        
        public Guid Id => Guid.TryParse(_id, out var guid) ? guid : Guid.Empty;
        public Sprite Picture => _picture;
        public string Name => _name;
        public bool IsPlayer => _isPlayer;

        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _picture;
        [SerializeField] private bool _isPlayer;
        
        public CharacterInfo Copy()
        {
            var copyCharacter = Instantiate(this);

            copyCharacter._id = _id;
            copyCharacter._name = _name;
            copyCharacter._picture = _picture;
            copyCharacter._isPlayer = _isPlayer;

            return copyCharacter;
        }
        
#if UNITY_EDITOR
        public void SetId(Guid id)
        {
            _id = id.ToString();
        }
        
        public void SetName(string newName)
        {
            _name = newName;
        }

        public void SetPicture(Sprite picture)
        {
            _picture = picture;
        }
        
        public void SetIsPlayer(bool isPlayer)
        {
            _isPlayer = isPlayer;
        }
#endif
    }
}