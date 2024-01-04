using System;
using System.Linq;
using PuzzlePlatformer.litescript_two.IO;
using PuzzlePlatformer.litescript_two.Nodes;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using NodeType = PuzzlePlatformer.litescript_two.Nodes.NodeType;
using ValueType = PuzzlePlatformer.litescript_two.Runtime.Values.ValueType;

namespace PuzzlePlatformer.litescript_two.Runtime;

/* TODO:
 * Handle lhs of assignment expression being a member expression.
 */

public class Interpreter(ErrorReporter reporter)
{
    private ErrorReporter Reporter { get; } = reporter;

    public IRuntimeValue Evaluate(IStatementNode node, Env env)
    {
        switch (node.Type)
        {
            case NodeType.IdentifierNode:
                return EvaluateIdentifier(node as IdentifierNode ?? throw new InvalidOperationException(), env);
            
            case NodeType.CallExpressionNode:
                return EvaluateCallExpression(node as CallExpressionNode ?? throw new InvalidOperationException(), env);
            
            case NodeType.AssignmentExpressionNode:
                return EvaluateAssignment(node as AssignmentExpressionNode ?? throw new InvalidOperationException(),
                    env);
            
            case NodeType.MemberExpressionNode:
                return EvaluateMemberExpression(node as MemberExpressionNode ?? throw new InvalidOperationException(),
                    env);
            
            case NodeType.IfStatementNode:
                return EvaluateIfStatement(node as IfStatementNode ?? throw new InvalidOperationException(), env);
            
            case NodeType.WhileStatementNode:
                return EvaluateWhileStatement(node as WhileStatementNode ?? throw new InvalidOperationException(), env);
            
            case NodeType.BinaryExpressionNode:
                return EvaluateBinaryExpression(node as BinaryExpressionNode ?? throw new InvalidOperationException(),
                    env);
            
            case NodeType.ProgramNode:
                return EvaluateProgram(node as ProgramNode ?? throw new InvalidOperationException(), env);
            
            case NodeType.VariableDeclarationNode:
                return EvaluateVariableDeclaration(
                    node as VariableDeclarationNode ?? throw new InvalidOperationException(), env);
            
            case NodeType.IntLiteralNode:
                if (node is IntLiteralNode intNode)
                    return new NumberValue(intNode.Value);
                break;
            
            case NodeType.StringLiteralNode:
                if (node is StringLiteralNode strNode)
                    return new StringValue(strNode.Value);
                break;
        }

        return new NullValue();
    }

    private bool IsTrue(IRuntimeValue condition)
    {
        return condition.Type switch
        {
            ValueType.Boolean => (condition as BooleanValue)!.Value,
            ValueType.Null => false,
            ValueType.String => (condition as StringValue)!.Value != "",
            ValueType.Number => (condition as NumberValue)!.Value != 0,
            _ => false
        };
    }

    private IRuntimeValue EvaluateIfStatement(IfStatementNode node, Env env)
    {
        var conditional = Evaluate(node.Condition, env);

        if (IsTrue(conditional))
            foreach (var statement in node.Consequent)
                Evaluate(statement, env);
        else
            foreach (var statement in node.Alternate)
                Evaluate(statement, env);

        return new NullValue();
    }

    private IRuntimeValue EvaluateWhileStatement(WhileStatementNode node, Env env)
    {
        var conditional = Evaluate(node.Condition, env);
        var counter = 0;

        while (IsTrue(conditional))
        {
            counter++;
            if (counter > 1000)
            {
                Reporter.RecordError(node.Position, "Whoops! The loop is infinite!");
                break;
            }
            
            foreach (var statement in node.Body)
                Evaluate(statement, env);
            
            conditional = Evaluate(node.Condition, env);
        }

        return new NullValue();
    }

    private IRuntimeValue EvaluateMemberExpression(MemberExpressionNode node, Env env)
    {
        if (node.Object is not IdentifierNode objIdent)
        {
            Reporter.RecordError(node.Position, "Null object identifier.");
            return new NullValue();
        }
        
        if (env.LookupVar(objIdent.Symbol) is not ObjectValue objValue)
        {
            Reporter.RecordError(node.Position, "Null object value.");
            return new NullValue();
        }
        
        if (node.Property is not IdentifierNode propertyIdent)
        {
            Reporter.RecordError(node.Position, "Null property value.");
            return new NullValue();
        }

        var propertySymbol = propertyIdent.Symbol;
        return objValue.Properties[propertySymbol];
    }

    private IRuntimeValue EvaluateCallExpression(CallExpressionNode node, Env env)
    {
        var args = node.Args.Select(arg => Evaluate(arg, env)).ToList();
        var function = (NativeFunctionValue)Evaluate(node.Caller, env);
        
        if(function.Type != ValueType.NativeFunction)
            Reporter.RecordError(node.Position, $"Cannot call value that is not a function: {function}");

        var result = function.Call(args, env);
        return result;
    }

    private IRuntimeValue EvaluateAssignment(AssignmentExpressionNode node, Env env)
    {
        var assigneeType = node.Identifier.Type;
        
        if (assigneeType != NodeType.IdentifierNode && assigneeType != NodeType.MemberExpressionNode)
            Reporter.RecordError(node.Position, $"Invalid identifier in assignment expression {node.Identifier}.");

        if (assigneeType == NodeType.IdentifierNode)
        {
            var varName = (node.Identifier as IdentifierNode)?.Symbol;
            if (varName != null)
                return env.AssignVariable(varName, Evaluate(node.Expression, env));
            
            Reporter.RecordError(node.Position, "Variable name cannot be null.");
        }
        
        // Member expression assignment
        var memberExpression = node.Identifier as MemberExpressionNode;

        if (memberExpression?.Object is not IdentifierNode objIdent)
        {
            Reporter.RecordError(node.Position, "Null object identifier.");
            return new NullValue();
        }
        
        if(env.LookupVar(objIdent.Symbol) is not ObjectValue objValue )
        {
            Reporter.RecordError(node.Position, "Null object value.");
            return new NullValue();
        }

        if (memberExpression.Property is not IdentifierNode propertyIdent)
        {
            Reporter.RecordError(node.Position, "Null property value.");
            return new NullValue();
        }

        var propertySymbol = propertyIdent.Symbol;

        return objValue.Properties[propertySymbol] = Evaluate(node.Expression, env);
    }

    private IRuntimeValue EvaluateVariableDeclaration(VariableDeclarationNode node, Env env)
    {
        var value = node.Value == null ? new NullValue() : Evaluate(node.Value, env);
        return env.DeclareVariable(node.Identifier, value, node.Constant);
    }

    private IRuntimeValue EvaluateIdentifier(IdentifierNode node, Env env)
    {
        return env.LookupVar(node.Symbol);
    }

    private IRuntimeValue EvaluateBinaryExpression(BinaryExpressionNode node, Env env)
    {
        var left = Evaluate(node.Left, env);
        var right = Evaluate(node.Right, env);

        if (node.Operator == "==")
            return EvaluateEqualityBinaryExpression(left, right, node.Position);

        if (node.Operator is "<" or ">")
            return EvaluateRelationalBinaryExpression(left, right, node.Operator, node.Position);
        
        if(left.Type == ValueType.Number && right.Type == ValueType.Number)
            return EvaluateNumericBinaryExpression(left as NumberValue ?? throw new InvalidOperationException(),
                right as NumberValue ?? throw new InvalidOperationException(), node.Operator);

        return new NullValue();
    }

    private BooleanValue EvaluateRelationalBinaryExpression(IRuntimeValue left, IRuntimeValue right, string op,
        Position position)
    {
        if (left.Type != ValueType.Number || right.Type != ValueType.Number)
        {
            Reporter.RecordError(position, "Relational expression can only contain numbers.");
            return new BooleanValue(false);
        }

        var leftValue = ((NumberValue)left).Value;
        var rightValue = ((NumberValue)right).Value;

        var result = op switch
        {
            "<" => leftValue < rightValue,
            ">" => leftValue > rightValue,
            _ => false
        };

        return new BooleanValue(result);
    }

    private BooleanValue EvaluateEqualityBinaryExpression(IRuntimeValue left, IRuntimeValue right, Position position)
    {
        if (left.Type != right.Type)
        {
            Reporter.RecordError(position, "Cannot check equality of differing types.");
            return new BooleanValue(false);
        }

        var value = left.Type switch
        {
            ValueType.Boolean => (left as BooleanValue)!.Value == (right as BooleanValue)!.Value,
            ValueType.Number => Math.Abs((left as NumberValue)!.Value - (right as NumberValue)!.Value) < 0.000001,
            ValueType.String => (left as StringValue)!.Value == (right as StringValue)!.Value,
            ValueType.Null => true,
            _ => false
        };

        return new BooleanValue(value);
    }

    private NumberValue EvaluateNumericBinaryExpression(NumberValue left, NumberValue right, string op)
    {
        var result = op switch
        {
            "+" => left.Value + right.Value,
            "-" => left.Value - right.Value,
            "*" => left.Value * right.Value,
            "/" => left.Value / right.Value,
            _ => 0
        };

        return new NumberValue(result);
    }

    private IRuntimeValue EvaluateProgram(ProgramNode node, Env env)
    {
        IRuntimeValue lastEvaluated = new NullValue();

        foreach (var statement in node.Body)
            lastEvaluated = Evaluate(statement, env);

        return lastEvaluated;
    }
}