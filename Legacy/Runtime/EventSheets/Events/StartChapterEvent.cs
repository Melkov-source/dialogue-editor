using System;

namespace Sandbox.Scripts.EventSheets.Events
{
    public class StartChapterEvent : EventBase
    {
        public override EventType Type => EventType.StartChapter;
        
        public Guid ChapterId;
        
        public class Parameter
        {
            public Guid Id;
        }
    }
}