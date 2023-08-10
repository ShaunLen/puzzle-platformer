using System.Collections.Generic;

namespace PuzzlePlatformer.litescript.Statements;

public class IfStatement : IExpression
{
    public NodeType Type { get; set; }
    public IExpression Condition;
    public List<IStatement> Consequent;
    public List<IStatement> Alternate;

    public IfStatement(IExpression condition, List<IStatement> consequent, List<IStatement> alternate)
    {
        Type = NodeType.IfStatement;
        Condition = condition;
        Consequent = consequent;
        Alternate = alternate;
    }
}