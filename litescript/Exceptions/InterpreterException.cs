using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class InterpreterException : Exception
{
    public InterpreterException(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("Interpreter Error: " + message);
        CodeManager.Instance.ConsoleWriteError(message);
    }
}