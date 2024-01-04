namespace PuzzlePlatformer.litescript_two.Nodes;

public class AssignmentExpressionNode(IExpressionNode identifier, IExpressionNode expression) : IExpressionNode
{
    public IExpressionNode Identifier { get; } = identifier;
    public IExpressionNode Expression { get; } = expression;
    public Position Position => Identifier.Position;
    public NodeType Type => NodeType.AssignmentExpressionNode;
}