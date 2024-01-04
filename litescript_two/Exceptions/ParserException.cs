using System;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.IO;

namespace PuzzlePlatformer.litescript_two.Exceptions;

public class ParserException : Exception
{
    public ParserException(ErrorReporter reporter, Position position, string errorMessage)
    {
        reporter.RecordError(position, errorMessage);

        foreach (var error in reporter.ErrorMessages)
            CodeManager.Instance.ConsoleWriteError(error, false);
        
        if(!CodeManager.Instance.EditorOpen)
            HudManager.Instance.WriteErrorNotification("Error: See console for details.");
    }
}