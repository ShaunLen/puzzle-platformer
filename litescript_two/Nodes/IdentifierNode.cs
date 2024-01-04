namespace PuzzlePlatformer.litescript_two.Nodes;

public class IdentifierNode(string symbol, Position position) : IExpressionNode
{
    public Position Position { get; }
    public readonly string Symbol = symbol;
    public NodeType Type => NodeType.IdentifierNode;
}