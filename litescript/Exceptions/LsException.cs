using System;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript.Exceptions;

public class LsException : Exception
{
    public LsException(string type, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(type + " Error: " + message);
        CodeManager.Instance.ConsoleWriteError(message);
        
        if(!CodeManager.Instance.EditorOpen)
            HudManager.Instance.WriteErrorNotification("Error: See console for details.");
    }
}