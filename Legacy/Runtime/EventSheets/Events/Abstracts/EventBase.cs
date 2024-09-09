using System.Collections.Generic;
using Sandbox.Scripts.EventSheets.Actions;

namespace Sandbox.Scripts.EventSheets.Events
{
    public abstract class EventBase
    {
        public abstract EventType Type { get; }
        public List<ActionBase> Actions = new();
    }
}