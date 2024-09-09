namespace Sandbox.Dialogue
{
    [DialogParameterMeta
    (
        Category = "Blackboard",
        Description = "Condition for equals blackboard value in table",
        Name = "Blackboard.Value",
        Type = DialogParameterType.CONDITION,
        ConcreteType = DialogConditionType.BLACKBOARD_VALUE
    )]
    public class BlackboardValueDialogCondition : DialogParameterBase
    {
        public override object Execute(object[] args)
        {
            return true;
        }
    }
}