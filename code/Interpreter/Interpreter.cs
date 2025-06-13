using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.AccessControl;

public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
{
    private object Evaluate(Expression expr)
    {
        return expr.Accept(this);
    }
    public object VisitLiteralExpression(LiteralExpression expression)
    {
        return expression.Value;
    }
    public object VisitGroupingExpression(GroupingExpression expression)
    {
        return Evaluate(expression);
    }
    public object VisitUnaryExpression(UnaryExpression expression)
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
    public void VisitLabelStatement(LabelStatement label) { } // labels don't need to be evaluated 

    public object VisitBinaryExpression(BinaryExpression expression)
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