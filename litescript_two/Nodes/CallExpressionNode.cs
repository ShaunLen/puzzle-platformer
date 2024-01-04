using System.Collections.Generic;

namespace PuzzlePlatformer.litescript_two.Nodes;

public class CallExpressionNode(IExpressionNode caller, List<IExpressionNode> args ) : IExpressionNode
{
    public IExpressionNode Caller { get; } = caller;
    public List<IExpressionNode> Args { get; } = args;
    public Position Position { get; }
    public NodeType Type => NodeType.CallExpressionNode;
}