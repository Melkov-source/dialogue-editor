using System;
using System.Runtime.Serialization;
using Sandbox.Scripts.Database;
using Sandbox.Scripts.Database.Enums;
using Sandbox.Scripts.EventSheets;
using Sandbox.Scripts.EventSheets.Events;
using UnityEngine;

namespace Sandbox.Scripts.Chapters
{
    public class Chapter : ScriptableObjectDataBase
    {
        public override DataType DataType => DataType.CHAPTER;
        public Guid Id => Guid.TryParse(_id, out var value) ? value : Guid.Empty;

        [SerializeField] private string _id;
        
        public void Start()
        {
            EventSheet.Notify<StartChapterEvent>(new StartChapterEvent.Parameter
            {
                Id = Id
            });
        }
    }
}