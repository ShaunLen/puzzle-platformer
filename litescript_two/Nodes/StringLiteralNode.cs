using System.Runtime.InteropServices;

namespace PuzzlePlatformer.litescript_two.Nodes;

public class StringLiteralNode(string value, Position position) : IExpressionNode
{
    public string Value { get; } = value;
    public Position Position { get; } = position;
    public NodeType Type => NodeType.StringLiteralNode;
}