using System.Collections.Generic;
using System.Resources;
using System.Text;
using PuzzlePlatformer.litescript_two.IO;

namespace PuzzlePlatformer.litescript_two.Tokenization;

public class Tokenizer(ErrorReporter reporter, TextReader reader)
{
    private ErrorReporter Reporter { get; } = reporter;
    private TextReader Reader { get; } = reader;
    private StringBuilder TokenSpelling { get; } = new();
    
    /* Public Methods */

    public List<Token> GetAllTokens()
    {
        var tokens = new List<Token>();
        var token = GetNextToken();
        while (token.Type != TokenType.Eof)
        {
            tokens.Add(token);
            token = GetNextToken();
        }
        
        tokens.Add(token);
        Reader.Close();
        return tokens;
    }
    
    /* Private Methods */

    private Token GetNextToken()
    {
        SkipSeparators();

        var tokenStartPosition = Reader.CurrentPosition;
        var tokenType = ScanToken();
        var token = new Token(tokenType, TokenSpelling.ToString(), tokenStartPosition);

        return token;
    }

    private void SkipSeparators()
    {
        while (Reader.Current == '#' || Reader.Current.IsWhiteSpace())
        {
            if (Reader.Current == '#')
                Reader.SkipRestOfLine();
            else
                Reader.MoveNext();
        }
    }

    private TokenType ScanToken()
    {
        TokenSpelling.Clear();

        if (Reader.Current == default(char))
        {
            Consume();
            return TokenType.Eof;
        }

        if (char.IsLetter(Reader.Current))
        {
            var startPosition = Reader.CurrentPosition;

            Consume();
            while (char.IsLetterOrDigit(Reader.Current))
                Consume();

            return TokenTypes.IsKeyword(TokenSpelling) 
                ? TokenTypes.GetTokenForKeyword(TokenSpelling) 
                : TokenType.Identifier;
        }

        if (char.IsDigit(Reader.Current))
        {
            Consume();
            while (char.IsDigit(Reader.Current))
                Consume();

            return TokenType.IntLiteral;
        }

        if (Reader.Current.IsOperator())
        {
            Consume();
            return TokenType.Operator;
        }

        switch (Reader.Current)
        {
            case '(':
                Consume();
                return TokenType.LeftParen;
            
            case ')':
                Consume();
                return TokenType.RightParen;
            
            case '{':
                Consume();
                return TokenType.LeftBrace;
            
            case '}':
                Consume();
                return TokenType.RightBrace;
            
            case '=':
                Consume();
                if (Reader.Current != '=') return TokenType.Becomes;
                Consume();
                return TokenType.Equal;

            case ';':
                Consume();
                while (Reader.Current == ';') Consume();
                return TokenType.SemiColon;
            
            case '.':
                Consume();
                return TokenType.Dot;
            
            case ',':
                Consume();
                return TokenType.Comma;
            
            case '\"':
                Reader.MoveNext();
                while (Reader.Current != '\"')
                    Consume();

                if (Reader.Current != '\"')
                {
                    Reporter.RecordError(Reader.CurrentPosition, "Expected closing quote after string.");
                    return TokenType.Error;
                }
                
                Reader.MoveNext();
                return TokenType.StringLiteral;
        }
        
        Reporter.RecordError(Reader.CurrentPosition, "Unexpected character.");
        Consume();
        return TokenType.Error;
    }

    private void Consume()
    {
        TokenSpelling.Append(Reader.Current);
        Reader.MoveNext();
    }
}

public static class ExtensionMethods
{
    public static bool IsWhiteSpace(this char c)
    {
        return c is ' ' or '\t' or '\n';
    }

    public static bool IsOperator(this char c)
    {
        switch (c)
        {
            case '+':
            case '-':
            case '*':
            case '/':
            case '<':
            case '>':
            case '!':
                return true;
            default:
                return false;
        }
    }
}