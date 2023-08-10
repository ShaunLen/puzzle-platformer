using System.Collections.Generic;

namespace PuzzlePlatformer.litescript.Statements;

public class Script : IStatement
{
    public NodeType Type { get; set; }
    
    public List<IStatement> Body;
    
    public Script(List<IStatement> body)
    {
        Type = NodeType.Script;
        Body = body;
    }

    public override string ToString()
    {
        return "{ " + Type + " : " + string.Join(", ", Body) + " }";
    }

}