using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class InterpreterException : LsException
{
    public InterpreterException(string message) : base("Environment", message)
    {
    }
}