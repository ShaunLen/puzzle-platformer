using System;
using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript_two.Nodes;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class ObjectPropertyRequirement : Requirement
{
    public override sealed string Desc { get; set; }
    public override bool Required { get; set; }
    private string _object, _property;
    private bool _isMethod;

    public ObjectPropertyRequirement(string obj, string property, bool required, bool isMethod = false)
    {
        if(isMethod)   
            Desc = "Call the '" + property + "()' method of the '" + obj + "' object.";
        else
            Desc = "Access the '" + property + "' property of the '" + obj + "' object.";
        
        _object = obj;
        _property = property;
        Required = required;
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
                    var memberExprs = GetMemberExpressionsIfStatement(stmt as IfStatementNode);
                    if (memberExprs.Any(MemberExpressionMatch))
                        return true;

                    continue;
                }
                case NodeType.WhileStatementNode:
                {
                    var memberExprs = GetMemberExpressionsWhileStatement(stmt as WhileStatementNode);
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

    private List<MemberExpressionNode> GetMemberExpressionsIfStatement(IfStatementNode ifStmt)
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
        
        var statements = ifStmt.Consequent.Concat(ifStmt.Alternate);
        
        foreach (var stmt in statements)
        {
            switch (stmt.Type)
            {
                case NodeType.WhileStatementNode:
                    memberExpressions.AddRange(GetMemberExpressionsWhileStatement(stmt as WhileStatementNode));
                    break;
                
                case NodeType.IfStatementNode:
                    memberExpressions.AddRange(GetMemberExpressionsIfStatement(stmt as IfStatementNode));
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
    
    private List<MemberExpressionNode> GetMemberExpressionsWhileStatement(WhileStatementNode whileStmt)
    {
        var memberExpressions = new List<MemberExpressionNode>();
        
        switch (whileStmt.Condition.Type)
        {
            case NodeType.MemberExpressionNode:
                memberExpressions.Add(whileStmt.Condition as MemberExpressionNode);
                break;
            
            case NodeType.CallExpressionNode:
            {
                var callExpr = whileStmt.Condition as CallExpressionNode;
                
                if(callExpr!.Caller.Type == NodeType.MemberExpressionNode)
                    memberExpressions.Add(callExpr.Caller as MemberExpressionNode);

                break;
            }
        }
        
        foreach (var stmt in whileStmt.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.WhileStatementNode:
                    memberExpressions.AddRange(GetMemberExpressionsWhileStatement(stmt as WhileStatementNode));
                    break;
                
                case NodeType.IfStatementNode:
                    memberExpressions.AddRange(GetMemberExpressionsIfStatement(stmt as IfStatementNode));
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