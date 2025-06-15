public abstract class Expression
{
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}

public class BinaryExpression : Expression
{
    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }
    public BinaryExpression(Expression left, Token op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }
}
public class UnaryExpression : Expression
{
    public Token Operator { get; }
    public Expression Expression { get; }
    public UnaryExpression(Token op, Expression expression)
    {
        Operator = op;
        Expression = expression;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }
}
public class LiteralExpression : Expression
{
    public object Value { get; }
    public Token Token { get; }
    public LiteralExpression(Token token)
    {
        Token = token;
        Value = Token.ParsedValue!;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}
public class GroupingExpression : Expression
{
    public Expression Expression;
    public GroupingExpression(Expression expression)
    {
        Expression = expression;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpression(this);
    }
}
public class VarExpression : Expression
{
    public string ID { get; }
    public Token Token { get;  }
    public VarExpression(Token token)
    {
        Token = token;
        ID = token.Value;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.VisitVarExpression(this);
    }
}
