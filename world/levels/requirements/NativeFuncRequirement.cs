using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript_two.Nodes;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class NativeFuncRequirement : Requirement
{
    public override sealed string Desc { get; set; }

    private string _methodName;

    public NativeFuncRequirement(string methodName)
    {
        Desc = "Call the native function '" + methodName + "()'.";
        _methodName = methodName;
    }

    public override bool RequirementMet(ProgramNode program)
    {
        foreach (var stmt in program.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                {
                    var callExpressions = GetCallExpressions(stmt as IfStatementNode);

                    if (callExpressions.Any(NativeFuncMatch))
                        return true;
                } break;

                case NodeType.CallExpressionNode:
                {
                    if (NativeFuncMatch(stmt as CallExpressionNode))
                        return true;
                } break;
            }
            
        }

        base.RequirementMet(program);
        return false;
    }

    private List<CallExpressionNode> GetCallExpressions(IfStatementNode ifStmt)
    {
        var callExpressions = new List<CallExpressionNode>();

        if(ifStmt.Condition.Type == NodeType.CallExpressionNode)
            callExpressions.Add(ifStmt.Condition as CallExpressionNode);

        var statements = ifStmt.Consequent.Concat(ifStmt.Alternate);

        foreach (var stmt in statements)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                    var callExprs = GetCallExpressions(stmt as IfStatementNode);
                    callExpressions.AddRange(callExprs);
                    break;
                
                case NodeType.CallExpressionNode:
                    callExpressions.Add(stmt as CallExpressionNode);
                    break;
            }
        }

        return callExpressions;
    }

    private bool NativeFuncMatch(CallExpressionNode expr)
    {
        if (expr!.Caller.Type != NodeType.IdentifierNode)
            return false;

        return ((IdentifierNode) expr.Caller).Symbol == _methodName;
    }
}