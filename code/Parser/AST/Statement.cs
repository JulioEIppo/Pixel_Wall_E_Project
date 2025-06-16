using System.Reflection.Emit;

public abstract class Statement
{
    public abstract T Accept<T>(IStatementVisitor<T> visitor);
}
public class ExpressionStatement : Statement
{
    public Expression Expression { get; }

    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class LabelStatement : Statement
{
    public string LabelID { get; }
    public int Line { get; }
    public LabelStatement(string labelID, int line)
    {
        LabelID = labelID;
        Line = line;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class GoToStatement : Statement
{
    public string Label { get; }
    public Expression Condition { get; }
    public GoToStatement(string label, Expression condition)
    {
        Label = label;
        Condition = condition;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class VarDeclaration : Statement
{
    public string ID { get; }
    public Expression Expression { get; }
    public VarDeclaration(string iD, Expression expression)
    {
        ID = iD;
        Expression = expression;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class SpawnStmt : Statement
{
    public Token Keyword { get; }
    public Expression X { get; }
    public Expression Y { get; }
    public SpawnStmt(Token keyword, Expression x, Expression y)
    {
        Keyword = keyword;
        X = x;
        Y = y;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class ColorStmt : Statement
{
    public Token Keyword { get; }
    public Expression Color { get; }
    public ColorStmt(Token keyword, Expression color)
    {
        Keyword = keyword;
        Color = color;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class SizeStmt : Statement
{
    public Token Keyword { get; }
    public Expression Size { get; }
    public SizeStmt(Token keyword, Expression size)
    {
        Keyword = keyword;
        Size = size;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class DrawLineStmt : Statement
{
    public Token Keyword { get; }
    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Distance { get; }

    public DrawLineStmt(Token keyword, Expression dirX, Expression dirY, Expression distance)
    {
        Keyword = keyword;
        DirX = dirY;
        DirY = dirY;
        Distance = distance;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class DrawCircleStmt : Statement
{
    public Token Keyword { get; }
    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Radius { get; }
    public DrawCircleStmt(Token keyword, Expression dirX, Expression dirY, Expression radius)
    {
        Keyword = keyword;
        DirX = dirX;
        DirY = dirY;
        Radius = radius;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}

public class DrawRectangleStmt : Statement
{
    public Token Keyword { get; }
    public int DirX { get; }
    public int DirY { get; }
    public int Distance { get; }
    public int Width { get; }
    public int Height { get; }

    public DrawRectangleStmt(Token keyword, int dirX, int dirY, int distance, int width, int height)
    {
        Keyword = keyword;
        DirX = dirX;
        DirY = dirY;
        Distance = distance;
        Width = width;
        Height = height;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
public class FillStmt : Statement
{
    public Token Keyword { get; }
    public FillStmt(Token keyword)
    {
        Keyword = keyword;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}