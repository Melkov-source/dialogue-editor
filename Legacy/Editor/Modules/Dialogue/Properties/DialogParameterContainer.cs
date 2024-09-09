using System;
using System.Collections.Generic;
using System.Linq;
using History.Editor.BrowserEditor;
using History.Editor.BrowserEditor.Window;
using History.Editor.UIToolkitExtensions;
using History.Tools;
using Sandbox.Dialogue;
using UnityEngine.UIElements;

namespace History.Editor.HistoryEditor.Dialogue.Properties
{
    public class DialogParameterContainer
    {
        public readonly VisualElement Root;

        private const string SCROLL_VIEW_CLASS = "container-scroll-view";
        private const string BUTTON_CREATE_NEW_CLASS = "button-create-new";

        private readonly ScrollView _scrollView;
        private readonly Button _createNewButton;

        private readonly Dictionary<DialogParameterInfo, DialogParameterElement> _parameterElements;

        private readonly DialogParameterType _parameterType;
        private readonly List<DialogParameterInfo> _parameters;

        public DialogParameterContainer(ref List<DialogParameterInfo> parameters, DialogParameterType parameterType, VisualElement root)
        {
            _parameterType = parameterType;
            _parameters = parameters;
            
            Root = root;

            _scrollView = root.Q<ScrollView>(className: SCROLL_VIEW_CLASS);
            _createNewButton = root.Q<Button>(className: BUTTON_CREATE_NEW_CLASS);

            _createNewButton.clicked += OnCreateNewParameterHandle;

            _parameterElements = new Dictionary<DialogParameterInfo, DialogParameterElement>();

            for (int index = 0, count = parameters.Count; index < count; index++)
            {
                var parameter = parameters[index];

                CreateParameter(parameter);
            }
        }

        private void OnCreateNewParameterHandle()
        {
            var parameter = new DialogParameterInfo
            {
                Type = 0,
                ParameterType = _parameterType,
            };

            _parameters.Add(parameter);

            CreateParameter(parameter);

            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
        
        private void CreateParameter(DialogParameterInfo parameter)
        {
            var element = new DialogParameterElement(parameter);

            element.OnDelete += OnDeleteParameterHandle;
            element.OnEdit += OnEditParameterHandle;

            _scrollView.Add(element.Root);

            _parameterElements.Add(parameter, element);
        }

        private void OnEditParameterHandle(DialogParameterInfo parameter)
        {
            if (IsDefaultType(parameter) == false)
            {
                EditParameterData(parameter);
                
                return;
            }
            
            EditParameterType(parameter);
        }
        
        private void EditParameterData(DialogParameterInfo parameter)
        {
            if (IsDefaultType(parameter))
            {
                return;
            }

            var window = DialogParameterSettingsWindow.Open();
                
            window.Setup(parameter, EditParameterType);
        }

        private void EditParameterType(DialogParameterInfo parameter)
        {
            var metas = DialogParameterMetaFinder.FindAllDialogParameterMeta(parameter.ParameterType);

            var categoryInfos = new Dictionary<string, CategoryInfo>();

            foreach (var meta in metas)
            {
                var iconType = meta.Icon.ParseEnum<SpriteType>();

                if (iconType == default)
                {
                    iconType = SpriteType.error_icon;
                }

                var childInfo = new CategoryChildInfo
                {
                    Description = meta.Description,
                    Name = meta.Name,
                    Icon = SpriteProvider.Get(iconType),
                    Data = meta.ConcreteType
                };

                if (categoryInfos.TryGetValue(meta.Category, out var categoryInfo) == false)
                {
                    categoryInfos.Add(meta.Category, new CategoryInfo
                    {
                        Name = meta.Category,
                        Children = { childInfo }
                    });

                    continue;
                }

                categoryInfo.Children.Add(childInfo);
            }

            Browser.OpenCategories(categoryInfos.Values.ToList(), SelectCategoryChildHandle);

            return;

            void SelectCategoryChildHandle(CategoryChildInfo info)
            {
                var actionType = (DialogActionType)info.Data;

                OnSelectParameter(parameter, (byte)actionType);
            }
        }
        
        private void OnSelectParameter(DialogParameterInfo parameter, byte concreteType)
        {
            parameter.Type = concreteType;
            parameter.Arguments = new List<object>();
            
            var element = _parameterElements[parameter];

            switch (parameter.ParameterType)
            {
                case DialogParameterType.ACTION:
                    var actionType = Enum.Parse<DialogActionType>(concreteType.ToString());
                    
                    element.SetName(actionType.ToString());
                    break;
                case DialogParameterType.CONDITION:
                    var conditionType =  Enum.Parse<DialogConditionType>(concreteType.ToString());
                    
                    element.SetName(conditionType.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }

            EditParameterData(parameter); 
        }

        private void OnDeleteParameterHandle(DialogParameterInfo parameter)
        {
            _parameters.Remove(parameter);

            var element = _parameterElements[parameter];
            _scrollView.Remove(element.Root);
            
            _parameterElements.Remove(parameter);

            if (HistoryEditorWindow.IsAutoSave)
            {
                HistoryEditorWindow.SaveCurrent();
            }
        }
        
        private bool IsDefaultType(DialogParameterInfo parameter)
        {
            switch (parameter.ParameterType)
            {
                case DialogParameterType.ACTION:
                    var actionType = Enum.Parse<DialogActionType>(parameter.Type.ToString());
                    return actionType == DialogActionType.EMPTY;
                
                case DialogParameterType.CONDITION:
                    var conditionType =  Enum.Parse<DialogConditionType>(parameter.Type.ToString());
                    return conditionType == DialogConditionType.EMPTY;
                
                default:
                    throw new Exception($"Not found target type for parameter: {parameter.Type}");
            }
        }
    }
}