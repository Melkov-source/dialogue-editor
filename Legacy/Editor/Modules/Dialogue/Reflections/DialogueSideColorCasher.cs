using System.Collections.Generic;
using History.Tools;
using Sandbox.Dialogue;
using UnityEngine;

namespace History.Editor.HistoryEditor.Dialogue.Reflections
{
    public static class DialogueSideColorMetaCacher
    {
        private static readonly Dictionary<DialogSide, ColorMetaAttribute> _colors = new();
        
        public static ColorMetaAttribute ParseColorMeta(DialogSide side)
        {
            if (_colors.TryGetValue(side, out var color))
            {
                return color;
            }
            
            color = side.GetAttribute<ColorMetaAttribute>();;

            _colors[side] = color;

            return color;
        }
    }
}