using System.Reflection.Emit;
namespace PixelWallE
{
    public abstract class Statement
    {
        public abstract void Accept<T>(IStatementVisitor<T> visitor);
    }
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
    public class LabelStatement : Statement
    {
        public Token LabelToken { get; }
        public string LabelID => LabelToken.Value;
        public int Line => LabelToken.Line;
        public LabelStatement(Token token)
        {
            LabelToken = token;
        }
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
    public class GoToStatement : Statement
    {
        public Token Keyword { get; }
        public string Label { get; }
        public Expression Condition { get; }
        public GoToStatement(Token keyword, string label, Expression condition)
        {
            Keyword = keyword;
            Label = label;
            Condition = condition;
        }
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
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
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
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
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
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
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
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
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
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
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
        }
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
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
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }

    public class DrawRectangleStmt : Statement
    {
        public Token Keyword { get; }
        public Expression DirX { get; }
        public Expression DirY { get; }
        public Expression Distance { get; }
        public Expression Width { get; }
        public Expression Height { get; }

        public DrawRectangleStmt(Token keyword, Expression dirX, Expression dirY, Expression distance, Expression width, Expression height)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
    public class FillStmt : Statement
    {
        public Token Keyword { get; }
        public FillStmt(Token keyword)
        {
            Keyword = keyword;
        }
        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}