namespace PuzzlePlatformer.litescript;

public enum NodeType
{
    // Statements
    Script,
    VariableDeclaration,
    IfStatement,
    
    // Expressions
    AssignmentExpression,
    MemberExpression,
    CallExpression,
    
    // Literals
    NumericLiteral,
    StringLiteral,
    Identifier,
    ObjectLiteral,
    BinaryExpression
}

public enum TokenType
{
    // Literals
    Number,
    Identifier,
    String,
    
    // Operators
    Equals,
    EqualsEquals,
    OpenParen,
    CloseParen,
    OpenBrace,
    CloseBrace,
    OpenSquareBracket,
    CloseSquareBracket,
    Colon,
    SemiColon,
    Comma,
    Dot,
    BinaryOperator,
    Eof,
    
    // Keywords
    Var,
    Const,
    If,
    Else
}

public enum ValueType
{
    Null,
    Number,
    String,
    Boolean,
    Object,
    NativeFunction
}