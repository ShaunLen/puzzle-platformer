namespace PuzzlePlatformer.litescript_two;

public class Position(int lineNumber, int positionInLine)
{
    public int LineNumber { get; } = lineNumber;
    public int PositionInLine { get; } = positionInLine;
    public static Position BuiltIn { get; } = new Position(-1, -1);

    public override string ToString()
    {
        return this == BuiltIn
            ? "System defined"
            : $"Line {LineNumber}, Column {PositionInLine}";
    }
}