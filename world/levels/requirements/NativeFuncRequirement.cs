using System;
using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript;
using PuzzlePlatformer.litescript.Statements;

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

    public override bool RequirementMet(Script script)
    {
        foreach (var stmt in script.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatement:
                {
                    var callExpressions = GetCallExpressions(stmt as IfStatement);

                    if (callExpressions.Any(NativeFuncMatch))
                        return true;
                } break;

                case NodeType.CallExpression:
                {
                    if (NativeFuncMatch(stmt as CallExpression))
                        return true;
                } break;
            }
            
        }

        base.RequirementMet(script);
        return false;
    }

    private List<CallExpression> GetCallExpressions(IfStatement ifStmt)
    {
        var callExpressions = new List<CallExpression>();

        if(ifStmt.Condition.Type == NodeType.CallExpression)
            callExpressions.Add(ifStmt.Condition as CallExpression);

        var statements = ifStmt.Consequent.Concat(ifStmt.Alternate);

        foreach (var stmt in statements)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatement:
                    var callExprs = GetCallExpressions(stmt as IfStatement);
                    callExpressions.AddRange(callExprs);
                    break;
                
                case NodeType.CallExpression:
                    callExpressions.Add(stmt as CallExpression);
                    break;
            }
        }

        return callExpressions;
    }

    private bool NativeFuncMatch(CallExpression expr)
    {
        if (expr!.Caller.Type != NodeType.Identifier)
            return false;

        return ((Identifier) expr.Caller).Symbol == _methodName;
    }
}