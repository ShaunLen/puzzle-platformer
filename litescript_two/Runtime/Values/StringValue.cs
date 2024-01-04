namespace PuzzlePlatformer.litescript_two.Runtime.Values;

public class StringValue(string value) : IRuntimeValue
{
    public ValueType Type => ValueType.String;
    public string Value { get; } = value;
}