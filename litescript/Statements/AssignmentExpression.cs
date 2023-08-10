using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class AssignmentExpression : IExpression
{
    public NodeType Type { get; set; }
    public IExpression Assignee;
    public IExpression Value;

    public AssignmentExpression(IExpression assignee, IExpression value)
    {
        Type = NodeType.AssignmentExpression;
        Assignee = assignee;
        Value = value;
    }
}