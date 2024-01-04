namespace PuzzlePlatformer.litescript_two.Nodes;

public class ErrorNode : IExpressionNode
{
    public Position Position { get; } = Position.BuiltIn;
    public NodeType Type => NodeType.ErrorNode;
}