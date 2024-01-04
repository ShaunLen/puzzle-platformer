namespace PuzzlePlatformer.litescript_two.Nodes;

public interface IAbstractSyntaxTreeNode
{
    Position Position { get; }
    NodeType Type { get; }
}