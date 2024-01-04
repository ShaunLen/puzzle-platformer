namespace PuzzlePlatformer.litescript_two.Nodes;

public class IntLiteralNode(int value, Position position) : IExpressionNode
{
    public int Value { get; } = value;
    public Position Position { get; } = position;
    public NodeType Type => NodeType.IntLiteralNode;
}