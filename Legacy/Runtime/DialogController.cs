using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Itibsoft.PanelManager;
using Sandbox.Scripts.InventorySystem.Utils;
using UnityEngine;
using Zenject;
using CharacterInfo = Sandbox.Scripts.Characters.CharacterInfo;

namespace Sandbox.Dialogue
{
    [Panel(PanelType = PanelType.Window, Order = 0, AssetId = "UI/Windows/DialogWindow")]
    public class DialogController : PanelControllerBase<DialogWindow>
    {
        public event Action<Dialog> OnOpenDialog;

        private bool _isWaitingNext;

        private bool _isWaitingReply;
        private ushort _replySelected;

        private Dictionary<DialogNodeType, DialogNodeProcessorBase> _nodeProcessors;
        
        [Inject] private readonly IInstantiator _instantiator;
        [Inject] private readonly InputService _input;

        protected override void Initialize()
        {
            Panel.Initialize();
            Panel.OnSelectedReply += OnReplyHandle;
            
            _nodeProcessors = new Dictionary<DialogNodeType, DialogNodeProcessorBase>();

            var executingAssembly = Assembly.GetExecutingAssembly();
            var types = executingAssembly.GetTypes();

            var parameter = new object[]
            {
                this
            };

            for (int index = 0, count = types.Length; index < count; index++)
            {
                var type = types[index];

                if (type.IsClass && type.BaseType == typeof(DialogNodeProcessorBase))
                {
                    var nodeProcessor = (DialogNodeProcessorBase)_instantiator.Instantiate(type, parameter);

                    _nodeProcessors.Add(nodeProcessor.Type, nodeProcessor);
                }
            }

            Panel.OnNext += OnNextHandle;

            _input.OnKeyDown += OnKeyDownHandle;
        }

        private void OnKeyDownHandle(KeyCode key)
        {
            if (key == KeyCode.Space)
            {
                OnNextHandle();
            }
        }

        public async Task ProcessDialog(Dialog dialog)
        {
            var root = dialog.Tree.Root;

            for (int index = 0, count = root.Nodes.Count; index < count; index++)
            {
                var node = root.Nodes[index];

                var result = await ProcessNode(node);

                if (result == false)
                {
                    return;
                }
            }
        }

        public async Task WaitNext()
        {
            _isWaitingNext = true;

            while (_isWaitingNext)
            {
                await Task.Yield();
            }
        }

        public async Task<ushort> WaitReply()
        {
            _isWaitingReply = true;

            while (_isWaitingReply)
            {
                await Task.Yield();
            }

            return _replySelected;
        }

        public void SetSentence(DialogCharacterInfo speaker, DialogCharacterInfo listener, string sentence)
        {
            Panel.SetSentence(speaker, listener, sentence);
        }

        public void SetChoice(DialogCharacterInfo speaker, DialogCharacterInfo listener, string sentence,
            Dictionary<ushort, string> replies)
        {
            SetSentence(speaker, listener, sentence);

            Panel.SetReplies(replies);
        }

        private async Task<bool> ProcessNode(DialogNodeBase node)
        {
            for (int index = 0, count = node.Actions.Count; index < count; index++)
            {
                var parameterInfo = node.Actions[index];

                var actionType = Enum.Parse<DialogActionType>(parameterInfo.Type.ToString());

                if (actionType == DialogActionType.DEBUG_LOG)
                {
                    var action = new DebugLogDialogAction();
                    action.Execute(parameterInfo.Arguments.ToArray());
                }
            }

            var nodeType = node.Type;

            var nodeProcessor = _nodeProcessors[nodeType];

            var resultNodes = await nodeProcessor.Process(node);

            for (int index = 0, count = resultNodes.Count; index < count; index++)
            {
                var resultNode = resultNodes[index];

                //TODO: Create simple logic for end dialog node (not implementation)
                if (resultNode.Type == DialogNodeType.END)
                {
                    return false; //Process end for current dialog
                }

                var result = await ProcessNode(resultNode);

                if (result == false)
                {
                    return false;
                }
            }

            return true; //Process end for this node
        }

        private void OnReplyHandle(ushort reply)
        {
            _replySelected = reply;
            _isWaitingReply = false;
        }

        private void OnNextHandle()
        {
            _isWaitingNext = false;
        }
    }
}