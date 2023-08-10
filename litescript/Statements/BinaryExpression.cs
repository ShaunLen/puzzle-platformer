using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class BinaryExpression : IExpression
{
    public NodeType Type { get; set; }
    
    public IExpression Left;
    public IExpression Right;
    public string Operator;

    public BinaryExpression(IExpression left, IExpression right, string @operator)
    {
        Type = NodeType.BinaryExpression;
        Left = left;
        Right = right;
        Operator = @operator;
    }
    
    public override string ToString()
    {
        return "{ type: " + Type + " : left: " + Left + ", right: " + Right + ", operator: " + Operator + " }";
    }

    
}