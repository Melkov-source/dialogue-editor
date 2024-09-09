using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sandbox.Dialogue
{
    public abstract class DialogNodeProcessorBase
    {
        public abstract DialogNodeType Type { get; }
        
        protected readonly DialogController Controller;

        protected DialogNodeProcessorBase(DialogController controller)
        {
            Controller = controller;
        }

        public abstract Task<List<DialogNodeBase>> Process(DialogNodeBase node);
    }
}