namespace PuzzlePlatformer.litescript_two.Runtime.Values;

public class BooleanValue(bool value) : IRuntimeValue
{
    public ValueType Type => ValueType.Boolean;
    public bool Value { get; } = value;
}