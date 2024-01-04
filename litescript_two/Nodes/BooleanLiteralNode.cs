namespace PuzzlePlatformer.litescript_two.Nodes;

public class BooleanLiteralNode(Position position, bool value) : IExpressionNode
{
    public bool Value { get; } = value;
    public Position Position { get; } = position;
    public NodeType Type => NodeType.BooleanLiteralNode;
}