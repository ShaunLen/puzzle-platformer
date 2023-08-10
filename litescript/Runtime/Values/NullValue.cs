using LiteScript;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public class NullValue : IRuntimeValue
{
    public ValueType Type { get; set; }
    public string? Value;

    public NullValue()
    {
        Type = ValueType.Null;
        Value = null;
    }
}