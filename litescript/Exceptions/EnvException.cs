using System;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.litescript.Exceptions;

public class EnvException : LsException
{
    public EnvException(string message) : base("Environment", message)
    {
    }
}