namespace PuzzlePlatformer.litescript_two.Nodes;

public class BinaryExpressionNode(IExpressionNode left, IExpressionNode right, string op) : IExpressionNode
{
    public IExpressionNode Left { get; } = left;
    public IExpressionNode Right { get; } = right;
    public string Operator { get; } = op;
    public Position Position => Left.Position;
    public NodeType Type => NodeType.BinaryExpressionNode;

    public override string ToString()
    {
        return $"BinaryExpressionNode ({left} {op} {right})";
    }
}