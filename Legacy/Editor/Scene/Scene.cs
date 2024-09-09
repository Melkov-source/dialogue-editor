using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using History.Editor.HistoryEditor.Blackboards;
using History.Editor.HistoryEditor.Characters;
using History.Editor.HistoryEditor.Dialogue;
using History.Editor.HistoryEditor.Items;
using History.Editor.HistoryEditor.Locations;
using History.Editor.HistoryEditor.Utils;
using History.Tools;
using History.Tools.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sandbox.Dialogue;
using Sandbox.Scripts.Blackboards;
using Sandbox.Scripts.Characters;
using Sandbox.Scripts.Database.Enums;
using Sandbox.Scripts.Items;
using Sandbox.Scripts.Locations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CharacterInfo = Sandbox.Scripts.Characters.CharacterInfo;

namespace History.Editor.HistoryEditor
{
    public class Scene : IDisposable
    {
        public readonly VisualElement Root;

        private const string SCENE_TAB_MENU_CLASS = "scene-tab-menu";
        private const string SCENE_CONTENT_CLASS = "scene-content";

        private readonly SceneTabMenu _sceneTabMenu;

        private readonly List<GUID> _files = new();
        private readonly Dictionary<GUID, SceneContextBase> _contexts = new();

        private readonly VisualElement _content;

        private SceneContextBase _currentContext;
        
        public Scene(VisualElement root)
        {
            Root = root;

            var sceneTabMenuView = root.Q<VisualElement>(className: SCENE_TAB_MENU_CLASS);
            _content = root.Q<VisualElement>(className: SCENE_CONTENT_CLASS);
            
            _sceneTabMenu = new SceneTabMenu(sceneTabMenuView);

            _sceneTabMenu.OnCloseTab += OnCloseTabHandle;
            _sceneTabMenu.OnSelectTab += OnSelectTabHandle;

            HistoryEditorWindow.OnSaveCurrent += OnSaveCurrentHandle;

            Explorer.OnOpenFile += OpenFile;
            Explorer.OnUpdateFile += UpdateFile;
            Explorer.OnDeleteFile += OnDeleteFile;
        }

        private void OpenFile(GUID assetId)
        {
            SelectFileContext(assetId);
            
            var searchedFile = _files.FirstOrDefault(info => info == assetId);

            if (searchedFile != default)
            {
                _sceneTabMenu.SelectFile(searchedFile);
                return;
            }
            
            _sceneTabMenu.AddTab(assetId);
            
            _files.Add(assetId);
        }
        
        private void UpdateFile(GUID assetId)
        {
            _sceneTabMenu.UpdateTab(assetId);
        }

        private void SetContext(SceneContextBase targetContext)
        {
            for (int index = 0, count = _contexts.Count; index < count; index++)
            {
                var context = _contexts.ElementAt(index).Value;

                if (context != targetContext)
                { 
                    context.Hide();
                }
                else
                {
                    context.Show();
                }
            }

            _currentContext = targetContext;
        }

        private void SelectFileContext(GUID assetId)
        {
            if (_contexts.TryGetValue(assetId, out var context))
            {
                SetContext(context);
                return;
            }
            
            var fileInfo = assetId.CreateFileInfo();
            var dataType = fileInfo.ParseDataType();

            switch (dataType)
            {
                case DataType.DIALOG:
                {
                    var json = fileInfo.ReadToEnd();
                    
                    var dialog = JsonConvert.DeserializeObject<Dialog>(json, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.All
                    });
                    
                    var dialogueContext = new DialogueSceneContext(dialog, assetId);
                    
                    _content.Add(dialogueContext.Root);
                    _contexts.Add(assetId, dialogueContext);
                    
                    SetContext(dialogueContext);
                    break;
                }
                case DataType.CHARACTER:
                {
                    var character = assetId.LoadAsset<CharacterInfo>();

                    var characterContext = new CharacterSceneContext(character, assetId);
                    
                    _content.Add(characterContext.Root);
                    _contexts.Add(assetId, characterContext);
                    
                    SetContext(characterContext);
                    break;
                }
                case DataType.LOCATION:
                {
                    var location = assetId.LoadAsset<Location>();

                    var locationContext = new LocationSceneContext(location, assetId);
                    
                    _content.Add(locationContext.Root);
                    _contexts.Add(assetId, locationContext);
                    
                    SetContext(locationContext);
                    
                    break;
                }
                case DataType.BLACKBOARD:
                {
                    var json = fileInfo.ReadToEnd();
                    
                    var blackboard = JsonConvert.DeserializeObject<Blackboard>(json);

                    var blackboardSceneContext = new BlackboardSceneContext(blackboard, assetId);
                    
                    _content.Add(blackboardSceneContext.Root);
                    _contexts.Add(assetId, blackboardSceneContext);
                    
                    SetContext(blackboardSceneContext);
                    
                    break;
                }
                case DataType.ITEM:
                {
                    var item = assetId.LoadAsset<Item>();

                    var characterContext = new ItemSceneContext(item, assetId);
                    
                    _content.Add(characterContext.Root);
                    _contexts.Add(assetId, characterContext);
                    
                    SetContext(characterContext);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SelectFirstFile()
        {
            if (_currentContext == default)
            {
                return;
            }
            
            if (_files.Count == 0)
            {
                _currentContext.Hide();
                Properties.CloseProperties();
                return;
            }

            var file = _files.First();
            
            OpenFile(file);
            
            Explorer.SelectFile(file);
        }
        
        
        private void OnDeleteFile(GUID assetId)
        {
            _files.Remove(assetId);

            _sceneTabMenu.RemoveTab(assetId);

            SelectFirstFile();
        }
        
        private void OnSelectTabHandle(GUID assetId)
        {
            SelectFileContext(assetId);
        }
        
        private void OnCloseTabHandle(GUID assetId)
        {
            _files.Remove(assetId);

            Explorer.UnselectFile(assetId);
            
            SelectFirstFile();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
        
        private void OnSaveCurrentHandle()
        {
            _currentContext.Save();
        }
    }
}