namespace PuzzlePlatformer.litescript_two.Nodes;

public class VariableDeclarationNode(bool constant, string identifier, IExpressionNode? value, Position position) : IStatementNode
{
    public bool Constant { get; } = constant;
    public string Identifier { get; } = identifier;
    public IExpressionNode? Value { get; } = value;
    public Position Position { get; } = position;
    public NodeType Type => NodeType.VariableDeclarationNode;
}