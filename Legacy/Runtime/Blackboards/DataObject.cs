using System;
using System.Runtime.Serialization;

namespace Sandbox.Scripts.Blackboards
{
    [DataContract]
    public sealed class DataObject
    {
        public Guid Id => _id;

        public string Key => _key;

        public object Value
        {
            get => _value;
            set => ChangeValue(value);
        }
        
        public event Action<object> OnChanged;

        [DataMember] private Guid _id;
        [DataMember] private string _key;
        [DataMember] private object _value;

        public DataObject(Guid id)
        {
            _id = id;
        }

        public void SetValueWithoutNotify(object newValue)
        {
            _value = newValue;
        }

        private void ChangeValue(object newValue)
        {
            _value = newValue;
            OnChanged?.Invoke(newValue);
        }

#if UNITY_EDITOR
        public void SetKey(string key)
        {
            _key = key;
        }

        public void SetValue(string value)
        {
            _value = value;
        }
#endif
    }
}