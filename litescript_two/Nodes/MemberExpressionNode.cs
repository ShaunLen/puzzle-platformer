namespace PuzzlePlatformer.litescript_two.Nodes;

public class MemberExpressionNode(IExpressionNode obj, IExpressionNode property) : IExpressionNode
{
    public IExpressionNode Object { get; } = obj;
    public IExpressionNode Property { get; } = property;
    public Position Position => Object.Position;
    public NodeType Type => NodeType.MemberExpressionNode;
}