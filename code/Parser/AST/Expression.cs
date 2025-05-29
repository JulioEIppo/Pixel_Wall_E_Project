public abstract class Expression
{
    public abstract T Accept<T>(IVisitor<T> visitor);
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
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }
}
public class UnaryExpression : Expression
{
    public Token Operator { get; }
    public Expression Expr { get; }
    public UnaryExpression(Token op, Expression expr)
    {
        Operator = op;
        Expr = expr;
    }
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }
}
public class LiteralExpression : Expression
{
    public object Value { get; }
    public LiteralExpression(object value)
    {
        Value = value;
    }
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}
public class IdentifierExpression : Expression
{
    public string Name { get; }
    public IdentifierExpression(string name)
    {
        Name = name;
    }
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIdentifierExpression(this);
    }
}
public class FunctionExpression : Expression
{
    public Token FunctionKeyword { get; }
    public List<Expression> Arguments { get; }
    public FunctionExpression(Token functionKeyword, List<Expression> arguments)
    {
        FunctionKeyword = functionKeyword;
        Arguments = arguments;
    }
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitFunctionExpression(this);
    }
}