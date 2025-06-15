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
        return visitor.VisitExpressionStatement(this);
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
        return visitor.VisitLabelStatement(this);
    }
}
public class GoToStatement : Statement
{
    public LabelStatement Label { get; }
    public Expression Condition { get; }
    public GoToStatement(LabelStatement label, Expression condition)
    {
        Label = label;
        Condition = condition;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitGoToStatement(this);
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
        return visitor.VisitVarDeclaration(this);
    }

}