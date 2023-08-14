using System;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.litescript.Exceptions;

public class EnvException : Exception
{
    public EnvException(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("Environment Error: " + message);
        CodeManager.Instance.ConsoleWriteError(message);
    }
}