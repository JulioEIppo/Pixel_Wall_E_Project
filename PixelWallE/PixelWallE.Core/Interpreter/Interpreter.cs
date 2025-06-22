using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.AccessControl;
using Avalonia.Utilities;

namespace PixeLWallE
{
    public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private Environment Environment = new();
        private Dictionary<string, INativeFunction> NativeFuncions = new();
        private LabelTable LabelTable = new();
        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }
        public void Visit(SpawnStmt spawn)
        {
            object x = Evaluate(spawn.X);
            object y = Evaluate(spawn.Y);
            if (x is int && y is int)
            {
                return;
            }
            throw new RuntimeErrorException(spawn.Keyword, "Expected int parameters");
        }
        public void Visit(ColorStmt color)
        {
            object colorString = Evaluate(color.Color);
            if (colorString is not string)
            {
                throw new RuntimeErrorException(color.Keyword, "Expected color");
            }
        }
        public object Visit(CallExpression expr)
        {
            if (!NativeFuncions.TryGetValue(expr.Function.Value, out var function))
                throw new RuntimeErrorException(expr.Function, $"Function: {expr.Function.Value}  not found");
            if (expr.Arguments.Count != function.Arity)
            {
                throw new RuntimeErrorException(expr.Function, $"Invalid amount of args, required arguments: {function.Arity}");
            }
            List<object> args = new();
            foreach (var arg in expr.Arguments)
            {
                args.Add(arg);
            }
            return function.Invoke(args);
        }
        public object Visit(VarDeclaration var)
        {
            object value = Evaluate(var.Expression);
            Environment.Define(var.ID, value);
            return value;
        }
        public object Visit(VarExpression var)
        {
            return Environment.Get(var.Token);
        }

        public object Visit(LiteralExpression expression)
        {
            return expression.Value;
        }
        public object Visit(GroupingExpression expression)
        {
            return Evaluate(expression);
        }
        public object Visit(UnaryExpression expression)
        {
            object right = Evaluate(expression.Expression);
            switch (expression.Operator.Type)
            {
                case TokenType.Subtract:
                    if (right is int rInt)
                    {
                        return -rInt;
                    }
                    throw new RuntimeErrorException(expression.Operator, "Integer operand required");
                case TokenType.Not:
                    if (right is bool rBool)
                    {
                        return !rBool;
                    }
                    throw new RuntimeErrorException(expression.Operator, "Boolean operand required");
                default: throw new RuntimeErrorException(expression.Operator, "Unknown operator");
            }
        }
        public object Visit(LabelStatement label) { return null!; } // labels don't need to be evaluated 
        public object Visit(ExpressionStatement statement)
        {
            return Evaluate(statement.Expression);
        }
        public object Visit(BinaryExpression expression)
        {
            object left = Evaluate(expression.Left);
            object right = Evaluate(expression.Right);
            switch (expression.Operator.Type)
            {
                case TokenType.Addition:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left + (int)right;

                case TokenType.Subtract:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left - (int)right;

                case TokenType.Multiplication:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left * (int)right;

                case TokenType.Division:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left / (int)right;

                case TokenType.Power:
                    CheckNumber(left, right, expression.Operator);
                    return HandlePower((int)left, (int)right, expression.Operator);

                case TokenType.Modulo:
                    CheckNumber(left, right, expression.Operator);
                    return HandleModulo((int)left, (int)right, expression.Operator);

                case TokenType.And:
                    var (aBool1, aBool2) = CheckBooleanOperands(left, right, expression.Operator);
                    return aBool1 && aBool2;

                case TokenType.Or:
                    var (oBool1, oBool2) = CheckBooleanOperands(left, right, expression.Operator);
                    return oBool1 || oBool2;

                case TokenType.Equal:
                    return IsEqual(left, right);

                case TokenType.NotEqual:
                    return !IsEqual(left, right);

                case TokenType.BiggerOrEqual:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left >= (int)right;

                case TokenType.Bigger:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left > (int)right;

                case TokenType.LesserOrEqual:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left <= (int)right;

                case TokenType.Lesser:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left < (int)right;

                default: throw new RuntimeErrorException(expression.Operator, "Unknown operator");
            }
        }
        private int HandlePower(int baseValue, int exponent, Token opToken)
        {
            if (exponent < 0)
            {
                throw new RuntimeErrorException(opToken, "Negative exponents are not supported");
            }
            if (baseValue == 0 && exponent == 0)
            {
                throw new RuntimeErrorException(opToken, "0^0 is not defined");
            }
            int result = 1;
            for (int i = 0; i < exponent; i++)
            {
                checked
                {
                    result *= baseValue;
                }
            }
            return result;
        }
        private void CheckNumber(object a, object b, Token opToken)
        {
            if (a is int && b is int) return;
            throw new RuntimeErrorException(opToken, "Operation requires integer operands");
        }
        private int HandleModulo(int a, int b, Token opToken)
        {
            if (b == 0)
            {
                throw new RuntimeErrorException(opToken, "Module by zero is not allowed");
            }
            return a % b;
        }
        private bool IsEqual(object a, object b)
        {
            return Equals(a, b);
        }
        private (bool, bool) CheckBooleanOperands(object a, object b, Token opToken)
        {
            if (a is bool lBool && b is bool rBool) return (lBool, rBool);
            throw new RuntimeErrorException(opToken, "Operation requires boolean operands");
        }

    }
}