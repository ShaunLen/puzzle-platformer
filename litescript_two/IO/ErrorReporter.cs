using System.Collections.Generic;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.ui;
using PuzzlePlatformer.ui.hud;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.litescript_two.IO;

public class ErrorReporter
{
    public bool HasErrors => NumberOfErrors > 0;
    public int NumberOfErrors { get; private set; }
    public HashSet<string> ErrorMessages { get; } = new();

    public void RecordError(Position position, string errorMessage)
    {
        NumberOfErrors++;
        if(position == Position.BuiltIn)
            ErrorMessages.Add($"[ERROR] " + errorMessage);
        else
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
        
        if (!UiManager.Instance.CodeInterfaceOpen)
            HudManager.Instance.SendNotification("Error: See console for details");

        return true;
    }
}