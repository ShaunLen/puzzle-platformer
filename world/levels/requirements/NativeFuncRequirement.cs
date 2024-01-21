using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript_two.Nodes;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class NativeFuncRequirement : Requirement
{
    public sealed override string Desc { get; set; }
    public sealed override bool Required { get; set; }

    private string _methodName;

    public NativeFuncRequirement(string methodName, bool required)
    {
        Desc = "Call the native function '" + methodName + "()'.";
        _methodName = methodName;
        Required = required;
    }

    public override bool RequirementMet(ProgramNode program)
    {
        foreach (var stmt in program.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                {
                    var callExpressions = GetCallExpressionsIfStatement(stmt as IfStatementNode);
                    if (callExpressions.Any(NativeFuncMatch))
                        return true;
                } break;
                
                case NodeType.WhileStatementNode:
                {
                    var callExpressions = GetCallExpressionsWhileStatement(stmt as WhileStatementNode);
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

    private List<CallExpressionNode> GetCallExpressionsIfStatement(IfStatementNode ifStmt)
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
                    callExpressions.AddRange(GetCallExpressionsIfStatement(stmt as IfStatementNode));
                    break;
                
                case NodeType.WhileStatementNode:
                    callExpressions.AddRange(GetCallExpressionsWhileStatement(stmt as WhileStatementNode));
                    break;
                
                case NodeType.CallExpressionNode:
                    callExpressions.Add(stmt as CallExpressionNode);
                    break;
            }
        }

        return callExpressions;
    }
    
    private List<CallExpressionNode> GetCallExpressionsWhileStatement(WhileStatementNode whileStmt)
    {
        var callExpressions = new List<CallExpressionNode>();

        if(whileStmt.Condition.Type == NodeType.CallExpressionNode)
            callExpressions.Add(whileStmt.Condition as CallExpressionNode);

        foreach (var stmt in whileStmt.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                    callExpressions.AddRange(GetCallExpressionsIfStatement(stmt as IfStatementNode));
                    break;
                
                case NodeType.WhileStatementNode:
                    callExpressions.AddRange(GetCallExpressionsWhileStatement(stmt as WhileStatementNode));
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