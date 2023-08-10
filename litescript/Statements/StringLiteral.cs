namespace PuzzlePlatformer.litescript.Statements;

public class StringLiteral : IExpression
{
    public NodeType Type { get; set; }
    public string Value;

    public StringLiteral(string value)
    {
        Type = NodeType.StringLiteral;
        Value = value;
    }
    
    public override string ToString()
    {
        return "{ " + Type + " : " + Value + " }";
    }
}