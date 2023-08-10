using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript;
using PuzzlePlatformer.litescript.Exceptions;

namespace LiteScript.Frontend;

/* TODO:
 * Track the position in input rather than removing char at 0 (more performant).
 * Implement "if" statement tokens.
 * Allow quotations to be escaped within string literals.
 */

public class Lexer
{
    // Keywords dictionary
    private readonly Godot.Collections.Dictionary<string, TokenType> _keywords = new()
    {
        {"if", TokenType.If},
        {"else", TokenType.Else},
        {"var", TokenType.Var},
        {"const", TokenType.Const}
    };

    public List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var src = input.ToCharArray().ToList();

        while (src.Count > 0)
        {
            switch (src[0])
            {
                case '(': tokens.AddToken(src, TokenType.OpenParen); break;
                case ')': tokens.AddToken(src, TokenType.CloseParen); break;
                case '{': tokens.AddToken(src, TokenType.OpenBrace); break;
                case '}': tokens.AddToken(src, TokenType.CloseBrace); break;
                case ':': tokens.AddToken(src, TokenType.Colon); break;
                case ';': tokens.AddToken(src, TokenType.SemiColon); break;
                case ',': tokens.AddToken(src, TokenType.Comma); break;
                case '.': tokens.AddToken(src, TokenType.Dot); break;
                case '[': tokens.AddToken(src, TokenType.OpenSquareBracket); break;
                case ']': tokens.AddToken(src, TokenType.CloseSquareBracket); break;
                case '+': 
                case '-': 
                case '*': 
                case '/': tokens.AddToken(src, TokenType.BinaryOperator); break;
                case '=':
                {
                    if (src[1] == '=')
                    {
                        tokens.AddLongToken(src[0].ToString() + src[1], TokenType.EqualsEquals);
                        src.RemoveAt(0);
                        src.RemoveAt(0);
                    }
                    else
                        tokens.AddToken(src, TokenType.Equals);
                } break;
                case '#':
                {
                    src.RemoveAt(0);
                    while(src.Count > 0 && src[0] != '\n' )
                        src.RemoveAt(0);
                } break;
                
                default:
                {
                    // Multi-character tokens
                    if (char.IsNumber(src[0]))
                    {
                        var num = "";

                        while (src.Count > 0 && char.IsNumber(src[0]))
                        {
                            num += src[0];
                            src.RemoveAt(0);
                        }
                        
                        tokens.AddLongToken(num, TokenType.Number);
                    }
                    else if (char.IsLetter(src[0]))
                    {
                        var ident = "";

                        while (src.Count > 0 && char.IsLetter(src[0]))
                        {
                            ident += src[0];
                            src.RemoveAt(0);
                        }

                        var tokenType = _keywords.TryGetValue(ident, out var keyword) ? keyword : TokenType.Identifier;
                        
                        tokens.AddLongToken(ident, tokenType);
                    }
                    else if (src[0].IsStringIdentifier())
                    {
                        src.RemoveAt(0);
                        var str = "";

                        while (src.Count > 0 && !src[0].IsStringIdentifier())
                        {
                            str += src[0];
                            src.RemoveAt(0);
                        }

                        if (src.Count > 0 && src[0].IsStringIdentifier())
                            src.RemoveAt(0);
                        else
                            throw new LexerException("Expected closing quotation at end of string: " + str);

                        tokens.AddLongToken(str, TokenType.String);
                    }
                    else if (src[0].IsSkippable())
                    {
                        src.RemoveAt(0);
                    }
                    else throw new LexerException("Unrecognised character found in input: " + src[0]);

                } break;
            }
        }
        
        tokens.AddLongToken("EndOfFile", TokenType.Eof);
        
        return tokens;
    }
}

public static class Extensions
{
    public static void AddToken(this List<Token> tokens, List<char> src, TokenType tokenType)
    {
        tokens.Add(new Token(src[0].ToString(), tokenType));
        src.RemoveAt(0);
    }

    public static void AddLongToken(this List<Token> tokens, string value, TokenType tokenType)
    {
        tokens.Add(new Token(value, tokenType));
    }

    public static bool IsSkippable(this char input)
    {
        var skippableChars = new List<char> { ' ', '\n', '\t', '\r' };
        return skippableChars.Contains(input);
    }

    public static bool IsStringIdentifier(this char input)
    {
        return input is '"';
    }
}