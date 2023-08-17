using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class ParserException : LsException
{
    public ParserException(string message) : base("Environment", message)
    {
    }
}