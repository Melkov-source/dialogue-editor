using System;
using System.Collections.Generic;
using History.Editor.HistoryEditor.Abstracts;
using History.Editor.HistoryEditor.Dialogue.Properties;
using Sandbox.Dialogue;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor
{
    public class Properties
    {
        public readonly VisualElement Root;

        private static Properties _instance;

        private const string CONTENT_CLASS = "properties-content";

        private readonly Header _header;
        private readonly VisualElement _content;

        private readonly Dictionary<DialogNodeBase, PropertiesGroupBase> _properties = new();

        private DialogNodeBase _currentDialogNode;

        public Properties(VisualElement root)
        {
            _instance = this;

            Root = root;

            var headerView = root.Q<VisualElement>(className: Header.ROOT_CLASS);
            _content = root.Q<VisualElement>(className: CONTENT_CLASS);

            _header = new Header(headerView);

            _header.SetTitle("Properties");
        }


        public static void OpenProperties(DialogNodeBase dialogNode)
        {
            if (_instance._properties.ContainsKey(dialogNode) == false)
            {
                switch (dialogNode.Type)
                {
                    case DialogNodeType.SENTENCE:
                        var sentenceProperties = new SentencePropertiesGroup((SentenceDialogNode)dialogNode);
                        _instance.AddPropertiesGroup(dialogNode, sentenceProperties);
                        break;
                    case DialogNodeType.CHOICE:
                        var choiceProperties = new SentencePropertiesGroup((ChoiceDialogNode)dialogNode);
                        _instance.AddPropertiesGroup(dialogNode, choiceProperties);
                        break;
                    case DialogNodeType.CHOICE_REPLY:
                        var replyProperties = new SentencePropertiesGroup((ReplyChoiceDialogNode)dialogNode);
                        _instance.AddPropertiesGroup(dialogNode, replyProperties);
                        break;
                    case DialogNodeType.RANDOM_BRANCH:
                        break;
                    case DialogNodeType.BRANCH:
                        break;
                    case DialogNodeType.END:
                        break;
                }
            }

            _instance.Select(dialogNode);
        }

        public static void CloseProperties()
        {
            if (_instance._currentDialogNode == default)
            {
                return;
            }

            _instance.Unselect(_instance._currentDialogNode);
        }

        private void Select(DialogNodeBase node)
        {
            foreach (var properties in _properties)
            {
                if (properties.Key != node)
                {
                    properties.Value.Close();
                }
                else
                {
                    properties.Value.Open();
                }
            }

            _currentDialogNode = node;
        }

        private void Unselect(DialogNodeBase node)
        {
            if (_currentDialogNode == default)
            {
                return;
            }
            
            if (_currentDialogNode != node)
            {
                return;
            }
            
            var properties = _instance._properties[_instance._currentDialogNode];
            properties.Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            _currentDialogNode = default;
        }

        private void AddPropertiesGroup(DialogNodeBase node, PropertiesGroupBase propertiesGroup)
        {
            _content.Add(propertiesGroup.Root);
            _properties.Add(node, propertiesGroup);
        }
    }
}