using System;
using System.Collections.Generic;
using PuzzlePlatformer.litescript_two.Exceptions;
using PuzzlePlatformer.litescript_two.IO;
using PuzzlePlatformer.litescript_two.Nodes;
using PuzzlePlatformer.litescript_two.Tokenization;

namespace PuzzlePlatformer.litescript_two;

public class Parser(ErrorReporter reporter)
{
    public ErrorReporter Reporter { get; } = reporter;
    private Token CurrentToken => _tokens[_currentIndex];
    private Token NextToken => _tokens[_currentIndex + 1];
    private List<Token> _tokens = null!;
    private int _currentIndex;

    public ProgramNode Parse(List<Token> tokens)
    {
        var program = new ProgramNode(new List<IStatementNode>());
        _tokens = tokens;

        while (NotEof())
        {
            program.Body.Add(ParseStatement());
        }

        return program;
    }
    
    private void MoveNext()
    {
        if (_currentIndex < _tokens.Count - 1)
            _currentIndex += 1;
    }
    
    private Token Accept(TokenType expectedType)
    {
        if (CurrentToken.Type != expectedType)
            Reporter.RecordError(CurrentToken.Position, $"Expected {expectedType}, got {CurrentToken.Type}.");

        var token = CurrentToken;
        MoveNext();
        return token;
    }

    private bool NotEof()
    {
        return CurrentToken.Type != TokenType.Eof;
    }

    private IStatementNode ParseStatement()
    {
        switch (CurrentToken.Type)
        {
            case TokenType.Var:
            case TokenType.Const:
                return ParseVariableDeclaration();
            
            default:
                return ParseExpression();
        }
    }

    private IStatementNode ParseVariableDeclaration()
    {
        var startPos = CurrentToken.Position;
        var isConstant = CurrentToken.Type == TokenType.Const;
        MoveNext();
        var identifier = Accept(TokenType.Identifier).Spelling;

        if (CurrentToken.Type == TokenType.SemiColon)
        {
            MoveNext();
            
            if(isConstant)
                Reporter.RecordError(CurrentToken.Position, "Constant declared without assigning value.");

            return new VariableDeclarationNode(isConstant, identifier, null, startPos);
        }

        Accept(TokenType.Becomes);
        var declaration = new VariableDeclarationNode(isConstant, identifier, ParseExpression(), startPos);
        Accept(TokenType.SemiColon);
        
        return declaration;
    }

    private IExpressionNode ParseIfStatement()
    {
        var startPos = CurrentToken.Position;
        MoveNext();
        var condition = ParseExpression();
        var consequent = new List<IStatementNode>();
        var alternate = new List<IStatementNode>();

        Accept(TokenType.LeftBrace);
        
        while(NotEof() && CurrentToken.Type != TokenType.RightBrace)
            consequent.Add(ParseExpression());

        Accept(TokenType.RightBrace);

        if (CurrentToken.Type == TokenType.Else)
            MoveNext();
        else
            return new IfStatementNode(startPos, condition, consequent, alternate);

        Accept(TokenType.LeftBrace);
        
        while(NotEof() && CurrentToken.Type != TokenType.RightBrace)
            alternate.Add(ParseExpression());

        Accept(TokenType.RightBrace);
        return new IfStatementNode(startPos, condition, consequent, alternate);
    }

    private IExpressionNode ParseWhileStatement()
    {
        var startPos = CurrentToken.Position;
        MoveNext();
        var condition = ParseExpression();
        var body = new List<IStatementNode>();

        Accept(TokenType.LeftBrace);
        
        while(NotEof() && CurrentToken.Type != TokenType.RightBrace)
            body.Add(ParseExpression());

        Accept(TokenType.RightBrace);

        return new WhileStatementNode(condition, body, startPos);
    }

    private IExpressionNode ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private IExpressionNode ParseAssignmentExpression()
    {
        var left = ParseAdditiveExpression();

        if (CurrentToken.Type != TokenType.Becomes)
            return left;
        
        MoveNext();

        var value = ParseAssignmentExpression();
        Accept(TokenType.SemiColon);
        
        return new AssignmentExpressionNode(left, value);
    }

    private IExpressionNode ParseAdditiveExpression()
    {
        var left = ParseMultiplicativeExpression();

        while (CurrentToken.Spelling is "+" or "-" or "==")
        {
            var op = CurrentToken.Spelling;
            MoveNext();
            var right = ParseMultiplicativeExpression();
            left = new BinaryExpressionNode(left, right, op);
        }

        return left;
    }
    
    private IExpressionNode ParseMultiplicativeExpression()
    {
        var left = ParseRelationalExpression();

        while (CurrentToken.Spelling is "*" or "/")
        {
            var op = CurrentToken.Spelling;
            MoveNext();
            var right = ParseCallMemberExpression();
            left = new BinaryExpressionNode(left, right, op);
        }

        return left;
    }
    
    private IExpressionNode ParseRelationalExpression()
    {
        var left = ParseCallMemberExpression();

        while (CurrentToken.Spelling is ">" or "<")
        {
            var op = CurrentToken.Spelling;
            MoveNext();
            var right = ParseCallMemberExpression();
            left = new BinaryExpressionNode(left, right, op);
        }

        return left;
    }

    private IExpressionNode ParseCallMemberExpression()
    {
        var member = ParseMemberExpression();

        return CurrentToken.Type == TokenType.LeftParen
            ? ParseCallExpression(member)
            : member;
    }

    private IExpressionNode ParseCallExpression(IExpressionNode caller)
    {
        var callExpression = new CallExpressionNode(caller, ParseArgs());

        if (CurrentToken.Type == TokenType.LeftParen)
            callExpression = ParseCallExpression(callExpression) as CallExpressionNode;
        
        // TODO: Temporarily disabled as causing issues with 'var x = foo.bar();' - it expects 2 semi-colons, both for function call and variable declaration
        Accept(TokenType.SemiColon);

        return callExpression ?? throw new InvalidOperationException();
    }

    private List<IExpressionNode> ParseArgs()
    {
        Accept(TokenType.LeftParen);
        var args = CurrentToken.Type == TokenType.RightParen ? new List<IExpressionNode>() : ParseArgsList();
        Accept(TokenType.RightParen);
        return args;
    }

    private List<IExpressionNode> ParseArgsList()
    {
        var args = new List<IExpressionNode> { ParseAssignmentExpression() };
        var consumed = false;
        
        while (CurrentToken.Type == TokenType.Comma)
        {
            if (!consumed)
            {
                MoveNext();
                consumed = true;
            }
            
            args.Add(ParseAssignmentExpression());
        }

        return args;
    }

    private IExpressionNode ParseMemberExpression()
    {
        var obj = ParsePrimaryExpression();

        while (CurrentToken.Type == TokenType.Dot)
        {
            MoveNext();
            var property = ParsePrimaryExpression();
            
            if(property is not IdentifierNode)
                Reporter.RecordError(CurrentToken.Position, "Expected an identifier.");

            return new MemberExpressionNode(obj, property);
        }

        return obj;
    }

    private IExpressionNode ParsePrimaryExpression()
    {
        var token = CurrentToken;
        
        switch (token.Type)
        {
            case TokenType.Identifier:
                MoveNext();
                return new IdentifierNode(token.Spelling, token.Position);
            
            case TokenType.IntLiteral:
                MoveNext();
                return new IntLiteralNode(int.Parse(token.Spelling), token.Position);
            
            case TokenType.StringLiteral:
                MoveNext();
                return new StringLiteralNode(token.Spelling, token.Position);
            
            case TokenType.LeftParen:
                MoveNext();
                var value = ParseExpression();
                Accept(TokenType.RightParen);
                return value;
            
            case TokenType.If:
                return ParseIfStatement();
            
            case TokenType.While:
                return ParseWhileStatement();
            
            default:
                throw new ParserException(Reporter, token.Position, "Unexpected character.");
        }
    }
}