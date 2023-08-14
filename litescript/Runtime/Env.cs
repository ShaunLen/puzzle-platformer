using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript.Exceptions;
using PuzzlePlatformer.litescript.Runtime.Values;
using PuzzlePlatformer.objects.interactable.terminal;

namespace PuzzlePlatformer.litescript.Runtime;

public class Env
{
    private Env? _parent;
    public Dictionary<string, IRuntimeValue> Variables;
    public HashSet<string> Constants;

    public Env(Env? parent)
    {
        _parent = parent;
        Variables = new Dictionary<string, IRuntimeValue>();
        Constants = new HashSet<string>();
    }

    public static Env CreateGlobalEnvironment()
    {
        var env = new Env(null);
        
        // Define native functions
        env.DeclareVariable("Print", new NativeFunctionValue(NativeFunctions.PrintNative), true);
        env.DeclareVariable("PrintLine", new NativeFunctionValue(NativeFunctions.PrintLineNative), true);
        
        env.DeclareVariable("true", new BooleanValue(true), true);
        env.DeclareVariable("false", new BooleanValue(false), true);
        env.DeclareVariable("null", new NullValue(), true);

        return env;
    }

    public IRuntimeValue DeclareVariable(string variableName, IRuntimeValue value, bool constant)
    {
        if (Variables.ContainsKey(variableName))
            throw new EnvException("Variable '" + variableName + "' is already defined.");
        
        Variables.Add(variableName, value);

        if (constant)
            Constants.Add(variableName);
        
        return value;
    }

    public IRuntimeValue AssignVariable(string variableName, IRuntimeValue value)
    {
        var env = Resolve(variableName);

        if (env.Constants.Contains(variableName))
            throw new EnvException("Cannot assign new value to constant: '" + variableName + "'");
        
        env.Variables[variableName] = value;

        return value;
    }

    public IRuntimeValue LookupVar(string variableName)
    {
        var env = Resolve(variableName);
        return env.Variables[variableName];
    }

    private Env Resolve(string variableName)
    {
        if (Variables.ContainsKey(variableName))
            return this;

        if (_parent == null)
            throw new EnvException("Cannot resolve '" + variableName + "' as it does not exist.");

        return _parent.Resolve(variableName);
    }
}