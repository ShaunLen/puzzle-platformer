using System;
using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript_two.Nodes;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class ObjectPropertyRequirement : Requirement
{
    public override sealed string Desc { get; set; }
    private string _object, _property;
    private bool _isMethod;

    public ObjectPropertyRequirement(string obj, string property, bool isMethod = false)
    {
        if(isMethod)   
            Desc = "Call the '" + property + "()' method of the '" + obj + "' object.";
        else
            Desc = "Access the '" + property + "' property of the '" + obj + "' object.";
        
        _object = obj;
        _property = property;
        _isMethod = isMethod;
    }

    public override bool RequirementMet(ProgramNode program)
    {
        MemberExpressionNode memberExpr = null;
        
        foreach (var stmt in program.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                {
                    var memberExprs = GetMemberExpressions(stmt as IfStatementNode);

                    if (memberExprs.Any(MemberExpressionMatch))
                        return true;

                    continue;
                }
                case NodeType.CallExpressionNode:
                {
                    var expr = stmt as CallExpressionNode;

                    if (expr!.Caller.Type != NodeType.MemberExpressionNode)
                        continue;
                
                    memberExpr = expr.Caller as MemberExpressionNode;
                    break;
                }
                case NodeType.MemberExpressionNode:
                    if (_isMethod)
                        continue;
                        
                    memberExpr = stmt as MemberExpressionNode;
                    break;
            }

            if(memberExpr is not { Type: NodeType.MemberExpressionNode })
                continue;

            Console.WriteLine(memberExpr.Type);

            if (MemberExpressionMatch(memberExpr))
                return true;
        }

        base.RequirementMet(program);
        return false;
    }

    private List<MemberExpressionNode> GetMemberExpressions(IfStatementNode ifStmt)
    {
        var memberExpressions = new List<MemberExpressionNode>();
        
        switch (ifStmt.Condition.Type)
        {
            case NodeType.MemberExpressionNode:
                memberExpressions.Add(ifStmt.Condition as MemberExpressionNode);
                break;
            
            case NodeType.CallExpressionNode:
            {
                var callExpr = ifStmt.Condition as CallExpressionNode;
                
                if(callExpr!.Caller.Type == NodeType.MemberExpressionNode)
                    memberExpressions.Add(callExpr.Caller as MemberExpressionNode);

                break;
            }
        }
        
        foreach (var stmt in ifStmt.Consequent)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                    var memberExprs = GetMemberExpressions(stmt as IfStatementNode);
                    memberExpressions.AddRange(memberExprs);
                    break;

                case NodeType.MemberExpressionNode:
                    memberExpressions.Add(stmt as MemberExpressionNode);
                    break;

                case NodeType.CallExpressionNode:
                    var callExpr = stmt as CallExpressionNode;
                    memberExpressions.Add(callExpr!.Caller as MemberExpressionNode);
                    break;
            }
        }
        
        foreach (var stmt in ifStmt.Alternate)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatementNode:
                    var memberExprs = GetMemberExpressions(stmt as IfStatementNode);
                    memberExpressions.AddRange(memberExprs);
                    break;

                case NodeType.MemberExpressionNode:
                    memberExpressions.Add(stmt as MemberExpressionNode);
                    break;
                
                case NodeType.CallExpressionNode:
                    var callExpr = stmt as CallExpressionNode;
                    memberExpressions.Add(callExpr!.Caller as MemberExpressionNode);
                    break;
            }
        }

        return memberExpressions;
    }

    private bool MemberExpressionMatch(MemberExpressionNode expr)
    {
        if (expr == null)
            return false;
        
        var obj = expr.Object as IdentifierNode;
        var property = expr.Property as IdentifierNode;
        
        return obj!.Symbol == _object && property!.Symbol == _property;
    }
}