using System.Collections.Generic;
using PuzzlePlatformer.litescript_two.Exceptions;
using PuzzlePlatformer.litescript_two.IO;
using PuzzlePlatformer.litescript_two.Runtime.Values;

namespace PuzzlePlatformer.litescript_two.Runtime;

public class Env(Env? parent)
{
    public Dictionary<string, IRuntimeValue> Variables { get; } = new();
    public HashSet<string> Constants { get; } = new();
    private Env? _parent = parent;

    public static Env CreateStandardEnvironment()
    {
        var env = new Env(null);
        
        // Define native functions
        env.DeclareVariable("Print", new NativeFunctionValue(NativeFunctions.PrintNative), true);
        env.DeclareVariable("PrintLine", new NativeFunctionValue(NativeFunctions.PrintLineNative), true);
        
        // Define native constants
        env.DeclareVariable("true", new BooleanValue(true), true);
        env.DeclareVariable("false", new BooleanValue(false), true);
        env.DeclareVariable("null", new NullValue(), true);

        return env;
    }

    public IRuntimeValue DeclareVariable(string varName, IRuntimeValue value, bool isConstant)
    {
        if (Variables.ContainsKey(varName))
            throw new EnvException($"Variable {varName} is already defined.");
        
        Variables.Add(varName, value);

        if (isConstant)
            Constants.Add(varName);

        return value;
    }

    public IRuntimeValue AssignVariable(string varName, IRuntimeValue value)
    {
        var env = Resolve(varName);

        if (env.Constants.Contains(varName))
            throw new EnvException($"Cannot assign new value to constant {varName}");

        env.Variables[varName] = value;
        return value;
    }

    public IRuntimeValue LookupVar(string varName)
    {
        var env = Resolve(varName);
        return env.Variables[varName];
    }

    public Env Resolve(string varName)
    {
        if (Variables.ContainsKey(varName))
            return this;
        
        if(_parent == null)
            throw new EnvException($"Cannot resolve {varName} as it does not exist.");

        return _parent.Resolve(varName);
    }
}