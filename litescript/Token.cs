namespace PuzzlePlatformer.litescript;

public class Token
{
    public readonly string Value;
    public readonly TokenType TokenType;

    public Token (string value, TokenType tokenType)
    {
        Value = value;
        TokenType = tokenType;
    }

    public override string ToString()
    {
        return "Token { " + Value + " : " + TokenType + " }";
    }
}