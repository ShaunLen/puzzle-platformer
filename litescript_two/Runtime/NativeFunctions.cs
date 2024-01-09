using System.Collections.Generic;
using System.Text;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.litescript_two.Runtime;

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
                
                case BooleanValue boolean:
                    output.Append(boolean.Value);
                    break;
            }
        }
        
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
                
                case BooleanValue boolean:
                    output.Append(boolean.Value);
                    break;
            }
        }
        
        GD.Print(output);
        CodeManager.Instance.ConsoleWriteLine(output.ToString());

        return new NullValue();
    }
}