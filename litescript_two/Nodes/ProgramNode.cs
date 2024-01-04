using System.Collections.Generic;

namespace PuzzlePlatformer.litescript_two.Nodes;

public class ProgramNode(List<IStatementNode> body) : IStatementNode
{
    public List<IStatementNode> Body = body;
    public Position Position => Position.BuiltIn;
    public NodeType Type => NodeType.ProgramNode;
}