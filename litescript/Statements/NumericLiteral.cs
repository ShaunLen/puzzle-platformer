using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class NumericLiteral : IExpression
{
    public NodeType Type { get; set; }
    public int Value;

    public NumericLiteral(int value)
    {
        Type = NodeType.NumericLiteral;
        Value = value;
    }

    public override string ToString()
    {
        return "{ " + Type + " : " + Value + " }";
    }
}