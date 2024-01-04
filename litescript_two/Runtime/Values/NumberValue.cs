namespace PuzzlePlatformer.litescript_two.Runtime.Values;

public class NumberValue(float value) : IRuntimeValue
{
    public ValueType Type => ValueType.Number;
    public float Value { get; } = value;
}