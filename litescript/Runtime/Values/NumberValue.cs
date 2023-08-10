using LiteScript;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public class NumberValue : IRuntimeValue
{
    public ValueType Type { get; set; }
    public readonly float Value;

    public NumberValue(float value)
    {
        Type = ValueType.Number;
        Value = value;
    }

    public override string ToString()
    {
        return "{ NumberValue : " + Value + " }";
    }
}