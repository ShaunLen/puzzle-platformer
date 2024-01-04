using System.Collections.Generic;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.litescript_two.IO;

public class ErrorReporter
{
    public bool HasErrors => NumberOfErrors > 0;
    public int NumberOfErrors { get; private set; }
    public HashSet<string> ErrorMessages { get; } = new();

    public void RecordError(Position position, string errorMessage)
    {
        NumberOfErrors++;
        ErrorMessages.Add($"[ERROR: Line {position.LineNumber}, Column {position.PositionInLine}] " + errorMessage);
    }

    public bool WriteErrorsIfAny()
    {
        if (!HasErrors)
            return false;
        
        foreach (var error in ErrorMessages)
        {
            CodeManager.Instance.ConsoleWriteError(error, false);
            CodeManager.Instance.ConsoleWriteLine("");
        }

        return true;
    }
}