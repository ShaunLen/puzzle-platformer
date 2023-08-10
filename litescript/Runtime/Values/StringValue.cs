using LiteScript;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public class StringValue : IRuntimeValue
{
    public ValueType Type { get; set; }
    public readonly string Value;

    public StringValue(string value)
    {
        Type = ValueType.String;
        Value = value;
    }
    
    public override string ToString()
    {
        return "{ StringValue : " + Value + " }";
    }
}