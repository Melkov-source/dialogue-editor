namespace Sandbox.Dialogue
{
    [DialogParameterMeta
    (
        Category = "Test",
        Name = "Test",
        Description = "Test Action...",
        Icon = "info_icon",
        Type = DialogParameterType.ACTION,
        ConcreteType = DialogActionType.EMPTY
    )]
    public class TestDialogAction : DialogParameterBase
    {
        public override object Execute(object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}