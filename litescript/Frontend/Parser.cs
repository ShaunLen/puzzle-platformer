using System;
using System.Collections.Generic;
using LiteScript;
using LiteScript.Frontend;
using PuzzlePlatformer.litescript.Exceptions;
using PuzzlePlatformer.litescript.Statements;

namespace PuzzlePlatformer.litescript.Frontend;

/* TODO:
 * Allow declaration of var/const inside if statement.
 */

public class Parser
{
    private List<Token> _tokens = new List<Token>();

    public Script ProduceAst(string src)
    {
        var lexer = new Lexer();
        var script = new Script(new List<IStatement>());
        
        _tokens = lexer.Tokenize(src);

        // Parse until end of file
        while (NotEof())
        {
            script.Body.Add(ParseStatement());
        }
        
        return script;
    }

    private bool NotEof()
    {
        return _tokens[0].TokenType != TokenType.Eof;
    }

    private Token CurrentToken()
    {
        return _tokens[0];
    }

    private Token ConsumeToken()
    {
        var token = CurrentToken();
        _tokens.RemoveAt(0);
        return token;
    }

    private Token ExpectToken(TokenType tokenType, string errorMsg)
    {
        var token = CurrentToken();
        _tokens.RemoveAt(0);

        if (token.TokenType != tokenType)
            throw new ParserException(errorMsg + " " + token + " - Expecting: " + tokenType);

        return token;
    }

    private IStatement ParseStatement()
    {
        switch (CurrentToken().TokenType)
        {
            case TokenType.Var:
            case TokenType.Const:
                return ParseVariableDeclaration();

            default:
                return ParseExpression();
        }
    }

    private IStatement ParseVariableDeclaration()
    {
        bool isConstant = ConsumeToken().TokenType == TokenType.Const;
        string identifier = ExpectToken(TokenType.Identifier, "Expected identifier name following variable declaration.").Value;
        VariableDeclaration declaration;
    
        if (CurrentToken().TokenType == TokenType.SemiColon)
        {
            ConsumeToken();
            
            if (isConstant)
                throw new ParserException("Constant declared without assigning value.");

            return new VariableDeclaration(isConstant, identifier, null);
        }

        ExpectToken(TokenType.Equals, "Expected semicolon or assignment operator following variable declaration.");

        declaration = new VariableDeclaration(isConstant, identifier, ParseExpression());

        ExpectToken(TokenType.SemiColon, "Expected semicolon following variable declaration.");

        return declaration;
    }

    private IExpression ParseIfStatement()
    {
        var condition = ParseExpression();
        var consequent = new List<IStatement>();
        var alternate = new List<IStatement>();

        ExpectToken(TokenType.OpenBrace, "Expected open brace after if condition.");

        while (NotEof() && CurrentToken().TokenType != TokenType.CloseBrace)
        {
            consequent.Add(ParseExpression());
        }
        
        ExpectToken(TokenType.CloseBrace, "If statement missing closing brace.");

        if (CurrentToken().TokenType == TokenType.Else)
            ConsumeToken();
        else
            return new IfStatement(condition, consequent, alternate);
        
        ExpectToken(TokenType.OpenBrace, "Expected open brace after 'else'.");
        
        while (NotEof() && CurrentToken().TokenType != TokenType.CloseBrace)
        {
            alternate.Add(ParseExpression());
        }
        
        ExpectToken(TokenType.CloseBrace, "Missing closing brace after 'else'.");

        return new IfStatement(condition, consequent, alternate );
    }

    private IExpression ParseExpression()
    {
        return ParseAssignmentExpression();
    }
    
    private IExpression ParseAssignmentExpression()
    {
        var left = ParseObjectExpression();

        if (CurrentToken().TokenType != TokenType.Equals)
            return left;

        ConsumeToken();
        
        // INSERT HERE
        
        var value = ParseAssignmentExpression();
        
        ExpectToken(TokenType.SemiColon, "Expected semicolon following assignment expression.");

        return new AssignmentExpression(left, value);
    }

    private IExpression ParseObjectExpression()
    {
        if (CurrentToken().TokenType != TokenType.OpenBrace)
            return ParseAdditiveExpression();

        ConsumeToken();

        var properties = new HashSet<Property>();

        while (NotEof() && CurrentToken().TokenType != TokenType.CloseBrace)
        {
            var key = ExpectToken(TokenType.Identifier, "Object literal key expected.").Value;

            // Allows shorthand key : pair -> { key, }
            if (CurrentToken().TokenType == TokenType.Comma)
            {
                ConsumeToken();
                properties.Add(new Property(key, null));
                continue;
            }

            // Allows shorthand key : pair -> { key }
            if (CurrentToken().TokenType == TokenType.CloseBrace)
            {
                properties.Add(new Property(key, null));
                continue;
            }

            ExpectToken(TokenType.Colon, "Expected colon following identifier in object expression.");
            var value = ParseExpression();

            properties.Add(new Property(key, value));

            if (CurrentToken().TokenType != TokenType.CloseBrace)
                ExpectToken(TokenType.Comma, "Expected comma or closing brace following property");
        }

        ExpectToken(TokenType.CloseBrace, "Object literal missing closing brace.");
        return new ObjectLiteral(properties);
    }

    private IExpression ParseAdditiveExpression()
    {
        var left = ParseMultiplicativeExpression();

        while (CurrentToken().Value == "+" || CurrentToken().Value == "-" || CurrentToken().Value == "==")
        {
            var op = ConsumeToken().Value;
            var right = ParseMultiplicativeExpression();
            left = new BinaryExpression(left, right, op);
        }

        return left;
    }
    
    private IExpression ParseMultiplicativeExpression()
    {
        var left = ParseCallMemberExpression();

        while (CurrentToken().Value == "*" || CurrentToken().Value == "/")
        {
            var op = ConsumeToken().Value;
            var right = ParseCallMemberExpression();
            left = new BinaryExpression(left, right, op);
        }

        return left;
    }

    private IExpression ParseCallMemberExpression()
    {
        var member = ParseMemberExpression();

        return CurrentToken().TokenType == TokenType.OpenParen ? // this is a function call
            ParseCallExpression(member) : member;
    }

    private IExpression ParseCallExpression(IExpression caller)
    {
        var callExpression = new CallExpression(caller, ParseArgs());

        if (CurrentToken().TokenType == TokenType.OpenParen)
            callExpression = ParseCallExpression(callExpression) as CallExpression;

        // TODO: Temporarily disabled as causing issues with 'var x = foo.bar();' - it expects 2 semi-colons, both for function call and variable declaration
        // ExpectToken(TokenType.SemiColon, "Expected semicolon following function call.");

        return callExpression ?? throw new InvalidOperationException();
    }

    private List<IExpression> ParseArgs()
    {
        ExpectToken(TokenType.OpenParen, "Expected open parenthesis.");

        var args = CurrentToken().TokenType == TokenType.CloseParen ? new List<IExpression>() : ParseArgsList();

        ExpectToken(TokenType.CloseParen, "Expected closing parenthesis inside arguments list.");

        return args;
    }

    private List<IExpression> ParseArgsList()
    {
        var args = new List<IExpression> { ParseAssignmentExpression() };
        var consumed = false;

        while (CurrentToken().TokenType == TokenType.Comma)
        {
            if (!consumed)
            {
                ConsumeToken();
                consumed = true;
            }
            
            args.Add(ParseAssignmentExpression());
        }

        return args;
    }

    private IExpression ParseMemberExpression()
    {
        var obj = ParsePrimaryExpression();

        while (CurrentToken().TokenType == TokenType.Dot || CurrentToken().TokenType == TokenType.OpenSquareBracket)
        {
            var op = ConsumeToken();
            
            // non-computed values - obj.expr
            IExpression property;
            bool computed;

            if (op.TokenType == TokenType.Dot)
            {
                computed = false;
                property = ParsePrimaryExpression(); // get identifier

                if (property.Type != NodeType.Identifier)
                    throw new ParserException("Dot operator must be followed by an identifier.");
            }
            else // computed values - obj[computedValue]
            {
                computed = true;
                property = ParseExpression();
                ExpectToken(TokenType.CloseSquareBracket, "Missing closing square bracket in computed value.");
            }

            obj = new MemberExpression(obj, property, computed);
        }

        return obj;
    }

    private IExpression ParsePrimaryExpression()
    {
        var token = CurrentToken();

        switch (token.TokenType)
        {
            case TokenType.Identifier: 
                return new Identifier(ConsumeToken().Value);
            
            case TokenType.Number: 
                return new NumericLiteral(int.Parse(ConsumeToken().Value));
            
            case TokenType.String:
                return new StringLiteral(ConsumeToken().Value);
            
            case TokenType.OpenParen:
                ConsumeToken(); // Consume opening parenthesis
                var value = ParseExpression();
                ExpectToken(TokenType.CloseParen, "Unexpected token found inside parenthesised expression. Expected closing parentheses."); // Consume closing parenthesis
                return value;
            
            case TokenType.If:
                ConsumeToken();
                return ParseIfStatement();
            
            default: 
                throw new ParserException("Unexpected token found during parsing: " + CurrentToken());
        }
    }
}