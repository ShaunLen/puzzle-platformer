using System;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.IO;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;
using HudManager = PuzzlePlatformer.ui.hud.HudManager;

namespace PuzzlePlatformer.litescript_two.Exceptions;

public class ParserException : Exception
{
    public ParserException(ErrorReporter reporter, Position position, string errorMessage)
    {
        reporter.RecordError(position, errorMessage);

        foreach (var error in reporter.ErrorMessages)
            CodeManager.Instance.ConsoleWriteError(error, false);
        
        if(!CodeManager.Instance.CodeInterfaceOpen)
            HudManager.Instance.SendNotification("Error: See console for details.", true);
    }
}