using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.UIToolkitExtensions
{
    public abstract class EditorView : IDisposable
    {
        public VisualElement Root { get; private set; }
        public IStyle Style => Root.style;

        private List<EditorView> _elements;

        public EditorView(VisualElement visualElement)
        {
            Root = visualElement;

            _elements = new List<EditorView>();
        }


        public abstract void Dispose();

        public static TEditorElement Create<TEditorElement>(VisualElement visualElement)
            where TEditorElement : EditorView
        {
            var type = typeof(TEditorElement);
            
            var instance = (TEditorElement)Activator.CreateInstance(type, visualElement);

            return instance;
        }
        
        public static TEditorElement Create<TEditorElement>(VisualElement visualElement, EditorStyle style)
            where TEditorElement : EditorView
        {
            var type = typeof(TEditorElement);
            
            var instance = (TEditorElement)Activator.CreateInstance(type, visualElement);
            
            instance.AddStyle(style);

            return instance;
        }

        public static TEditorElement Create<TEditorElement>(EditorTemplate editorTemplate, EditorStyle style = default)
            where TEditorElement : EditorView
        {
            var instance = editorTemplate.Create();

            if (style != default)
            {
                instance.styleSheets.Add(style.StyleSheet);
            }

            return Create<TEditorElement>(instance);
        }

        public void AddStyle(EditorStyle style)
        {
            Root.styleSheets.Add(style.StyleSheet);
        }

        public void AddStyleClass(string className)
        {
            Root.AddToClassList(className);
        }

        public void RemoveStyleClass(string className)
        {
            Root.RemoveFromClassList(className);
        }

        public void SetVisibility(bool visible)
        {
            Style.visibility = new StyleEnum<Visibility>(visible ? Visibility.Visible : Visibility.Hidden);
        }

        public void SetDisplay(bool isDisplay)
        {
            Style.display = new StyleEnum<DisplayStyle>(isDisplay ? DisplayStyle.Flex : DisplayStyle.None);
        }

        public void SetParent(EditorView editorView)
        {
            Root.Add(editorView.Root);
        }

        public void RemoveParent(EditorView editorView)
        {
            Root.Remove(editorView.Root);
        }

        public TEditorElement GetElement<TEditorElement>(string key, QueryType queryType)
            where TEditorElement : EditorView
        {
            var elementCashed = FindElementCashed<TEditorElement>(key, queryType);

            if (elementCashed != null)
            {
                return elementCashed;
            }

            var visualElement = queryType switch
            {
                QueryType.Class => Root.Q(className: key),
                QueryType.Name => Root.Q(name: key),

                _ => throw new ArgumentOutOfRangeException
                (
                    nameof(queryType),
                    queryType,
                    "Not found query type selection!"
                )
            };

            if (visualElement == null)
            {
                Debug.LogError("visualElement is null");
            }

            var editorElement = Create<TEditorElement>(visualElement);

            _elements.Add(editorElement);

            return editorElement;
        }

        private TEditorElement FindElementCashed<TEditorElement>(string key, QueryType queryType = QueryType.Class)
            where TEditorElement : EditorView
        {
            for (int index = 0, count = _elements.Count; index < count; index++)
            {
                var target = _elements[index];

                switch (queryType)
                {
                    case QueryType.Class:
                        var classes = target.Root.GetClasses();

                        if (classes.Any(@class => @class.StartsWith(key)))
                        {
                            return target as TEditorElement;
                        }

                        break;

                    case QueryType.Name:
                        if (target.Root.name == key)
                        {
                            return target as TEditorElement;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException
                        (
                            nameof(queryType),
                            queryType,
                            "Not found query type selection!"
                        );
                }
            }

            return null;
        }
    }
}