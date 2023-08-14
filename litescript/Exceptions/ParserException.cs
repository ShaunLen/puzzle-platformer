using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class ParserException : Exception
{
    public ParserException(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("Parser Error: " + message);
        CodeManager.Instance.ConsoleWriteError(message);
    }
}