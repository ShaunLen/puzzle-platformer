using LiteScript;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public class BooleanValue : IRuntimeValue
{
    public ValueType Type { get; set; }
    public bool Value;

    public BooleanValue(bool value)
    {
        Type = ValueType.Boolean;
        Value = value;
    }

    public override string ToString()
    {
        return "{ boolean : " + Value + " }";
    }
}