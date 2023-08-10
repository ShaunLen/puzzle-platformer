using System.Collections.Generic;

namespace PuzzlePlatformer.litescript.Statements;

public class ObjectLiteral : IExpression
{
    public NodeType Type { get; set; }
    public HashSet<Property> Properties;

    public ObjectLiteral(HashSet<Property> properties)
    {
        Type = NodeType.ObjectLiteral;
        Properties = properties;
    }
}