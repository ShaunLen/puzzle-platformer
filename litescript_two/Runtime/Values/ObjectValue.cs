using System.Collections.Generic;

namespace PuzzlePlatformer.litescript_two.Runtime.Values;

public class ObjectValue(Dictionary<string, IRuntimeValue> properties) : IRuntimeValue
{
    public ValueType Type => ValueType.Object;
    public Dictionary<string, IRuntimeValue> Properties { get; } = properties;
}