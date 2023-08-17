using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class LexerException : LsException
{
    public LexerException(string message) : base("Environment", message)
    {
    }
}