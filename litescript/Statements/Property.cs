namespace PuzzlePlatformer.litescript.Statements;

public class Property : IExpression
{
    public NodeType Type { get; set; }
    public string Key;
    public IExpression? Value; // ? means optional

    public Property(string key, IExpression? value)
    {
        Key = key;
        Value = value;
    }
}