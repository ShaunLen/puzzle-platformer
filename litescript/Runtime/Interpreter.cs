using System;
using System.Collections.Generic;
using System.Linq;
using LiteScript;
using PuzzlePlatformer.litescript.Exceptions;
using PuzzlePlatformer.litescript.Runtime.Values;
using PuzzlePlatformer.litescript.Statements;
using ValueType = PuzzlePlatformer.litescript.ValueType;

namespace PuzzlePlatformer.litescript.Runtime;

/* TODO:
 * Handle lhs of assignment expression being a member expression.
 * Handle evaluation of member expressions.
 */

public class Interpreter
{
    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="astNode">AST Node</param>
    /// <param name="env">Environment</param>
    /// <returns><c>IRuntimeValue</c> of relevant type.</returns>
    /// <exception cref="System.InvalidOperationException">Thrown when null reference is passed to evaluation method lower in stack.</exception>
    /// <exception cref="InterpreterException">Thrown when <see cref="astNode"/> has not been set up for interpretation.</exception>
    public IRuntimeValue Evaluate(IStatement astNode, Env env)
    {
        switch (astNode.Type)
        {
            case NodeType.Identifier:
                return EvaluateIdentifier(astNode as Identifier ?? throw new InvalidOperationException(), env);
            
            case NodeType.ObjectLiteral:
                return EvaluateObjectExpression(astNode as ObjectLiteral ?? throw new InvalidOperationException(), env);
            
            case NodeType.CallExpression:
                return EvaluateCallExpression(astNode as CallExpression ?? throw new InvalidOperationException(), env);
            
            case NodeType.AssignmentExpression:
                return EvaluateAssignment(astNode as AssignmentExpression ?? throw new InvalidOperationException(), env);
            
            case NodeType.MemberExpression:
                return EvaluateMemberExpression(astNode as MemberExpression ?? throw new InvalidOperationException(), env);
            
            case NodeType.IfStatement:
                return EvaluateIfStatement(astNode as IfStatement ?? throw new InvalidOperationException(), env);
            
            case NodeType.NumericLiteral:
                if (astNode is NumericLiteral numNode)
                    return new NumberValue(numNode.Value);
                break;
            
            case NodeType.StringLiteral:
                if (astNode is StringLiteral strNode)
                    return new StringValue(strNode.Value);
                break;
            
            case NodeType.BinaryExpression:
                return EvaluateBinaryExpression(astNode as BinaryExpression ?? throw new InvalidOperationException(), env);

            case NodeType.Script:
                return EvaluateScript(astNode as Script ?? throw new InvalidOperationException(), env);
            
            case NodeType.VariableDeclaration:
                return EvaluateVariableDeclaration(astNode as VariableDeclaration ?? throw new InvalidOperationException(), env);
            
            default:
                throw new InterpreterException("This AST Node has not yet been set up for interpretation: " + astNode.Type);
        }

        return new NullValue();
    }

    private IRuntimeValue EvaluateIfStatement(IfStatement astNode, Env env)
    {
        var conditional = Evaluate(astNode.Condition, env);

        if (IsTruthy(conditional))
        {
            foreach (var statement in astNode.Consequent)
            {
                Evaluate(statement, env);
            }
        }
        else
        {
            foreach (var statement in astNode.Alternate)
            {
                Evaluate(statement, env);
            }
        }

        return new NullValue();
    }

    private bool IsTruthy(IRuntimeValue condition)
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

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="objLiteral"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    private IRuntimeValue EvaluateObjectExpression(ObjectLiteral objLiteral, Env env)
    {
        var obj = new ObjectValue(new Dictionary<string, IRuntimeValue>());

        foreach (var property in objLiteral.Properties)
        {
            // Handles valid key: pair
            var runtimeValue = property.Value == null ? env.LookupVar(property.Key) : Evaluate(property.Value, env);
            obj.Properties.Add(property.Key, runtimeValue);
        }
        
        return obj;
    }

    private IRuntimeValue EvaluateMemberExpression(MemberExpression memberExpression, Env env)
    {
        if (memberExpression.Object is not Identifier objIdent)
            throw new InterpreterException("Null object identifier.");

        if (env.LookupVar(objIdent.Symbol) is not ObjectValue objValue)
            throw new InterpreterException("Null object value.");

        if (memberExpression.Property is not Identifier propertyIdent)
            throw new InterpreterException("Null property value.");
        
        var propertySymbol = propertyIdent.Symbol;

        return objValue.Properties[propertySymbol];
    }

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    private IRuntimeValue EvaluateCallExpression(CallExpression expression, Env env)
    {
        var args = expression.Args.Select(arg => Evaluate(arg, env)).ToList();
        var fn = (NativeFunctionValue) Evaluate(expression.Caller, env);

        if (fn.Type != ValueType.NativeFunction)
            throw new InterpreterException("Cannot call value that is not a function: " + fn);

        var result = fn.Call(args, env);

        return result;
    }
    

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="astNode"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    private IRuntimeValue EvaluateAssignment(AssignmentExpression astNode, Env env)
    {
        var assigneeType = astNode.Assignee.Type;
        
        if (assigneeType != NodeType.Identifier && assigneeType != NodeType.MemberExpression)
            throw new InterpreterException("Invalid left side of assignment expression: " + astNode.Assignee);

        if (assigneeType == NodeType.Identifier)
        {
            var variableName = (astNode.Assignee as Identifier)?.Symbol;
            if (variableName != null)
                return env.AssignVariable(variableName, Evaluate(astNode.Value, env));

            throw new InterpreterException("Variable name cannot be null");
        }

        // Member expression assignment
        var memberExpression = astNode.Assignee as MemberExpression;

        if (memberExpression?.Object is not Identifier objIdent)
            throw new InterpreterException("Null object identifier.");

        if (env.LookupVar(objIdent.Symbol) is not ObjectValue objValue)
            throw new InterpreterException("Null object value.");

        if (memberExpression.Property is not Identifier propertyIdent)
            throw new InterpreterException("Null property value.");
        
        var propertySymbol = propertyIdent.Symbol;

        return objValue.Properties[propertySymbol] = Evaluate(astNode.Value, env);
    }
    

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="env">Environment</param>
    /// <returns></returns>
    private IRuntimeValue EvaluateVariableDeclaration(VariableDeclaration declaration, Env env)
    {
        var value = declaration.Value == null ? new NullValue() : Evaluate(declaration.Value, env);

        return env.DeclareVariable(declaration.Identifier, value, declaration.Constant);
    }
    

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="env">Environment</param>
    /// <returns></returns>
    private IRuntimeValue EvaluateIdentifier(Identifier identifier, Env env)
    {
        return env.LookupVar(identifier.Symbol);
    }
    

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IRuntimeValue EvaluateBinaryExpression(BinaryExpression expression, Env env)
    {
        var left = Evaluate(expression.Left, env);
        var right = Evaluate(expression.Right, env);

        if (expression.Operator == "==")
            return EvaluateEqualityBinaryExpression(left, right);

        if (left.Type == ValueType.Number && right.Type == ValueType.Number)
            return EvaluateNumericBinaryExpression(left as NumberValue ?? throw new InvalidOperationException(),
                right as NumberValue ?? throw new InvalidOperationException(), expression.Operator);

        return new NullValue();
    }


    private BooleanValue EvaluateEqualityBinaryExpression(IRuntimeValue left, IRuntimeValue right)
    {
        if (left.Type != right.Type)
            throw new InterpreterException("Cannot check equality of differing types.");

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
    

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="op"></param>
    /// <returns></returns>
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
    

    /// <summary>
    /// To be completed.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    private IRuntimeValue EvaluateScript(Script script, Env env)
    {
        IRuntimeValue lastEvaluated = new NullValue();

        foreach (var statement in script.Body)
            lastEvaluated = Evaluate(statement, env);

        return lastEvaluated;
    }
}