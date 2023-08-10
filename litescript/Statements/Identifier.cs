using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public class Identifier : IExpression
{
    public NodeType Type { get; set; }
    public readonly string Symbol;

    public Identifier(string symbol)
    {
        Type = NodeType.Identifier;
        Symbol = symbol;
    }
    
    public override string ToString()
    {
        return "{ " + Type + " : " + Symbol + " }";
    }

}