namespace PuzzlePlatformer.litescript_two.Runtime.Values;

public class NullValue : IRuntimeValue
{
    public ValueType Type => ValueType.Null;
    public string Value => null;
}