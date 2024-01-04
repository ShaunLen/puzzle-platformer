namespace PuzzlePlatformer.litescript_two.Exceptions;

public class EnvException : LsException
{
    public EnvException(string message) : base("Environment", message)
    {
    }
}