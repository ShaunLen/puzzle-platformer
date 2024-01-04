using System.IO;

namespace PuzzlePlatformer.litescript_two.IO;

public class TextReader
{
    public StringReader Reader { get; }
    public char Current => AtEndOfFile ? default : _currentLine[_currentPositionInLine - 1];
    public Position CurrentPosition => new(_currentLineNumber, _currentPositionInLine);
    private string _currentLine;
    private int _currentLineNumber;
    private int _currentPositionInLine;

    public TextReader(string input)
    {
        Reader = new(input);
        ReadNextLine();;
    }

    private bool AtEndOfFile => _currentLine == null;
    

    public bool MoveNext()
    {
        if (AtEndOfFile) return false;

        if (_currentPositionInLine < _currentLine.Length)
            _currentPositionInLine++;
        else
            ReadNextLine();

        return true;
    }

    public char PeekNext()
    {
        if (AtEndOfFile || _currentPositionInLine >= _currentLine.Length)
            return default;

        return _currentLine[_currentPositionInLine];
    }

    public void SkipRestOfLine()
    {
        if (!AtEndOfFile)
            ReadNextLine();
    }

    public void Close()
    {
        Reader.Close();
    }

    private void ReadNextLine()
    {
        _currentLine = Reader.ReadLine();
        if (_currentLine != null)
            _currentLine += "\n";
        _currentLineNumber += 1;
        _currentPositionInLine = 1;
    }
}