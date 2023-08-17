using System.Collections.Generic;
using System.Linq;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript;
using PuzzlePlatformer.litescript.Statements;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class ObjectPropertyRequirement : Requirement
{
    public override sealed string Desc { get; set; }
    private string _object, _property;

    public ObjectPropertyRequirement(string obj, string property)
    {
        Desc = "Access the '" + property + "' property of the '" + obj + "' object.";
        _object = obj;
        _property = property;
    }

    public override bool RequirementMet(Script script)
    {
        foreach (var stmt in script.Body)
        {
            if (stmt.Type == NodeType.IfStatement)
            {
                var memberExprs = GetMemberExpressions(stmt as IfStatement);

                if (memberExprs.Any(MemberExpressionMatch))
                    return true;
            }

            if(stmt.Type != NodeType.MemberExpression)
                continue;

            if (MemberExpressionMatch(stmt as MemberExpression))
                return true;
        }

        base.RequirementMet(script);
        return false;
    }

    private List<MemberExpression> GetMemberExpressions(IfStatement ifStmt)
    {
        var memberExpressions = new List<MemberExpression>();
        
        if(ifStmt.Condition.Type == NodeType.MemberExpression)
            memberExpressions.Add(ifStmt.Condition as MemberExpression);
        
        foreach (var cons in ifStmt.Consequent)
        {
            switch (cons.Type)
            {
                case NodeType.IfStatement:
                    var memberExprs = GetMemberExpressions(cons as IfStatement);
                    memberExpressions.AddRange(memberExprs);
                    break;

                case NodeType.MemberExpression:
                    memberExpressions.Add(cons as MemberExpression);
                    break;
            }
        }
        
        foreach (var alt in ifStmt.Alternate)
        {
            switch (alt.Type)
            {
                case NodeType.IfStatement:
                    var memberExprs = GetMemberExpressions(alt as IfStatement);
                    memberExpressions.AddRange(memberExprs);
                    break;

                case NodeType.MemberExpression:
                    memberExpressions.Add(alt as MemberExpression);
                    break;
            }
        }

        return memberExpressions;
    }

    private bool MemberExpressionMatch(MemberExpression expr)
    {
        var obj = expr.Object as Identifier;
        var property = expr.Property as Identifier;
        
        return obj!.Symbol == _object && property!.Symbol == _property;
    }
}