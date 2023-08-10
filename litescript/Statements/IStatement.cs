using LiteScript;

namespace PuzzlePlatformer.litescript.Statements;

public interface IStatement
{
    public NodeType Type { get; set; }
}