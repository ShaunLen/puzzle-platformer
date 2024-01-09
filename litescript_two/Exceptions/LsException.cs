using System;
using PuzzlePlatformer.autoloads;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;
using HudManager = PuzzlePlatformer.ui.hud.HudManager;

namespace PuzzlePlatformer.litescript_two.Exceptions;

public class LsException : Exception
{
    public LsException(string type, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(type + " Error: " + message);
        CodeManager.Instance.ConsoleWriteError(message);
        
        if(!CodeManager.Instance.CodeInterfaceOpen)
            HudManager.Instance.SendNotification("Error: See console for details.", true);
    }
}