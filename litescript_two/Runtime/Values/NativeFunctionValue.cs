using System.Collections.Generic;

namespace PuzzlePlatformer.litescript_two.Runtime.Values;

public delegate IRuntimeValue FunctionCall(List<IRuntimeValue> args, Env env);

public class NativeFunctionValue(FunctionCall call) : IRuntimeValue
{
    public ValueType Type => ValueType.NativeFunction;
    public FunctionCall Call { get; } = call;
}