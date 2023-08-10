using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class BooleanLiteral : IExpression
{
    public NodeType Type { get; set; }
    public bool Value;
}