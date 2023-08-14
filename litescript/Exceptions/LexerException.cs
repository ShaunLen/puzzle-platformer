using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class LexerException : Exception
{
    public LexerException(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("Lexer Error: " + message);
        CodeManager.Instance.ConsoleWriteError(message);
    }
}