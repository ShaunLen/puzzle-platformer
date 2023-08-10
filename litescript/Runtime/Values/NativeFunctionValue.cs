using System.Collections.Generic;

namespace PuzzlePlatformer.litescript.Runtime.Values;

public delegate IRuntimeValue FunctionCall(List<IRuntimeValue> args, Env env);

public class NativeFunctionValue : IRuntimeValue
{
    public ValueType Type { get; set; }
    public FunctionCall Call;

    public NativeFunctionValue(FunctionCall call)
    {
        Type = ValueType.NativeFunction;
        Call = call;
    }
}