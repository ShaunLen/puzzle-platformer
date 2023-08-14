using System.Collections.Generic;
using System.Text;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript.Exceptions;
using PuzzlePlatformer.litescript.Runtime.Values;

namespace PuzzlePlatformer.litescript.Runtime;

public static class NativeFunctions
{
    public static IRuntimeValue PrintNative(List<IRuntimeValue> args, Env env)
    {
        var output = new StringBuilder();

        foreach (var arg in args)
        {
            switch (arg)
            {
                case NumberValue num:
                    output.Append(num.Value);
                    break;
                
                case StringValue str:
                    output.Append(str.Value);
                    break;
                
                case BooleanValue boo:
                    output.Append(boo.Value);
                    break;
                
                default:
                    throw new InterpreterException("Print method may only contain numbers and strings - type: " + arg.Type);
            }
        }
        
        GD.Print(output);
        CodeManager.Instance.ConsoleWrite(output.ToString());
        
        return new NullValue();
    }
    
    public static IRuntimeValue PrintLineNative(List<IRuntimeValue> args, Env env)
    {
        var output = new StringBuilder();

        foreach (var arg in args)
        {
            switch (arg)
            {
                case NumberValue num:
                    output.Append(num.Value);
                    break;
                
                case StringValue str:
                    output.Append(str.Value);
                    break;
                
                case BooleanValue boo:
                    output.Append(boo.Value);
                    break;
                
                default:
                    throw new InterpreterException("PrintLine method may only contain numbers and strings - type: " + arg.Type);
            }
        }
        
        return new NullValue();
    }
}