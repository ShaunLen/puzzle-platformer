namespace PuzzlePlatformer.litescript_two.Tokenization;

public class Token(TokenType type, string spelling, Position position)
{
    public TokenType Type { get; } = type;
    public string Spelling { get; } = spelling;
    public Position Position { get; } = position;

    public override string ToString()
    {
        return $"type={Type}, spelling=\"{Spelling}\", position={Position}";
    }
}