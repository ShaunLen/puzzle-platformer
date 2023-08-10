namespace PuzzlePlatformer.litescript.Statements;

public class VariableDeclaration : IStatement
{
    public NodeType Type { get; set; }
    public bool Constant;
    public string Identifier;
    public IExpression? Value;

    public VariableDeclaration(bool constant, string identifier, IExpression? value)
    {
        Constant = constant;
        Identifier = identifier;
        Value = value;
        Type = NodeType.VariableDeclaration;
    }
}