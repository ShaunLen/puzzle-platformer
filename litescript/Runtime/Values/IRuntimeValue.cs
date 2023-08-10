using LiteScript;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public interface IRuntimeValue
{
    public ValueType Type { get; set; }
}