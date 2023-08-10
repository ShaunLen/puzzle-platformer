using System.Collections.Generic;
using LiteScript;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public class ObjectValue : IRuntimeValue
{
    public ValueType Type { get; set; }
    public Dictionary<string, IRuntimeValue> Properties;

    public ObjectValue(Dictionary<string, IRuntimeValue> properties)
    {
        Type = ValueType.Object;
        Properties = properties;
    }
}