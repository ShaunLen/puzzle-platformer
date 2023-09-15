using System;
using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.litescript;
using PuzzlePlatformer.litescript.Statements;

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

    public override bool RequirementMet(Script script)
    {
        MemberExpression memberExpr = null;
        
        foreach (var stmt in script.Body)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatement:
                {
                    var memberExprs = GetMemberExpressions(stmt as IfStatement);

                    if (memberExprs.Any(MemberExpressionMatch))
                        return true;

                    continue;
                }
                case NodeType.CallExpression:
                {
                    var expr = stmt as CallExpression;

                    if (expr!.Caller.Type != NodeType.MemberExpression)
                        continue;
                
                    memberExpr = expr.Caller as MemberExpression;
                    break;
                }
                case NodeType.MemberExpression:
                    if (_isMethod)
                        continue;
                        
                    memberExpr = stmt as MemberExpression;
                    break;
            }

            if(memberExpr is not { Type: NodeType.MemberExpression })
                continue;

            Console.WriteLine(memberExpr.Type);

            if (MemberExpressionMatch(memberExpr))
                return true;
        }

        base.RequirementMet(script);
        return false;
    }

    private List<MemberExpression> GetMemberExpressions(IfStatement ifStmt)
    {
        var memberExpressions = new List<MemberExpression>();
        
        switch (ifStmt.Condition.Type)
        {
            case NodeType.MemberExpression:
                memberExpressions.Add(ifStmt.Condition as MemberExpression);
                break;
            
            case NodeType.CallExpression:
            {
                var callExpr = ifStmt.Condition as CallExpression;
                
                if(callExpr!.Caller.Type == NodeType.MemberExpression)
                    memberExpressions.Add(callExpr.Caller as MemberExpression);

                break;
            }
        }
        
        foreach (var stmt in ifStmt.Consequent)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatement:
                    var memberExprs = GetMemberExpressions(stmt as IfStatement);
                    memberExpressions.AddRange(memberExprs);
                    break;

                case NodeType.MemberExpression:
                    memberExpressions.Add(stmt as MemberExpression);
                    break;

                case NodeType.CallExpression:
                    var callExpr = stmt as CallExpression;
                    memberExpressions.Add(callExpr!.Caller as MemberExpression);
                    break;
            }
        }
        
        foreach (var stmt in ifStmt.Alternate)
        {
            switch (stmt.Type)
            {
                case NodeType.IfStatement:
                    var memberExprs = GetMemberExpressions(stmt as IfStatement);
                    memberExpressions.AddRange(memberExprs);
                    break;

                case NodeType.MemberExpression:
                    memberExpressions.Add(stmt as MemberExpression);
                    break;
                
                case NodeType.CallExpression:
                    var callExpr = stmt as CallExpression;
                    memberExpressions.Add(callExpr!.Caller as MemberExpression);
                    break;
            }
        }

        return memberExpressions;
    }

    private bool MemberExpressionMatch(MemberExpression expr)
    {
        if (expr == null)
            return false;
        
        var obj = expr.Object as Identifier;
        var property = expr.Property as Identifier;
        
        return obj!.Symbol == _object && property!.Symbol == _property;
    }
}