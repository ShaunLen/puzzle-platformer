using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static PuzzlePlatformer.litescript_two.Tokenization.TokenType;

namespace PuzzlePlatformer.litescript_two.Tokenization;

public enum TokenType
{
    // Non-terminals
    Identifier, IntLiteral, StringLiteral, Operator,
    
    // Terminals (reserved words)
    Var, Const, If, Else, While, ForEach,
    
    // Terminals (punctuation)
    LeftParen, RightParen, LeftBrace, RightBrace, Becomes, Equal, SemiColon, Dot, Comma,
    
    // Special Tokens
    Error, Eof
}

public static class TokenTypes
{
    private static ImmutableDictionary<string, TokenType> Keywords { get; } = new Dictionary<string, TokenType>()
    {
        { "var", Var },
        { "const", Const },
        { "if", If },
        { "else", Else },
        { "while", While },
        { "foreach", ForEach },
    }.ToImmutableDictionary();

    public static bool IsKeyword(StringBuilder word)
    {
        return Keywords.ContainsKey(word.ToString());
    }

    public static TokenType GetTokenForKeyword(StringBuilder word)
    {
        if (!IsKeyword((word))) throw new ArgumentException("Word is not a keyword.");
        return Keywords[word.ToString()];
    }
}