using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class MemberExpression : IExpression
{
    public NodeType Type { get; set; }
    public IExpression Object;
    public IExpression Property;
    public bool Computed;

    public MemberExpression(IExpression obj, IExpression property, bool computed)
    {
        Type = NodeType.MemberExpression;
        Object = obj;
        Property = property;
        Computed = computed;
    }
}