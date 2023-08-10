using System.Collections.Generic;
using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class CallExpression : IExpression
{
    public NodeType Type { get; set; }
    public List<IExpression> Args;
    public IExpression Caller;

    public CallExpression(IExpression caller, List<IExpression> args)
    {
        Type = NodeType.CallExpression;
        Caller = caller;
        Args = args;
    }
}