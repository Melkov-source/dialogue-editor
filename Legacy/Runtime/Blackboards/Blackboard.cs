using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sandbox.Scripts.Database;
using Sandbox.Scripts.Database.Enums;

namespace Sandbox.Scripts.Blackboards
{
    [DataContract]
    public class Blackboard : DataBase
    {
        [DataMember] public override DataType DataType => DataType.BLACKBOARD;
        
        private Guid _id;

        [DataMember] private List<DataObject> _objects = new();


        public DataObject GetObject(string key)
        {
            return _objects.First(@object => @object.Key == key);
        }

#if UNITY_EDITOR

        public List<DataObject> GetAllDataObjects()
        {
            return _objects;
        }

        public void AddDataObject(DataObject dataObject)
        {
            _objects.Add(dataObject);
        }

        public void RemoveDataObject(DataObject dataObject)
        {
            _objects.Remove(dataObject);
        }
#endif
    }
}