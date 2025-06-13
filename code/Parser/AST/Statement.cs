using System.Reflection.Emit;

public abstract class Statement
{
    public abstract T Accept<T>(IStatementVisitor<T> visitor);
}
public class LabelStatement : Statement
{
    public string LabelID { get; }
    public LabelStatement(string labelID)
    {
        LabelID = labelID;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitLabelStatement(this);
    }
}
public class AssignmentStatement : Statement
{
    public Token Identifier { get; }
    public Token AssignOperator { get; }
    public Expression Value { get; }
    public AssignmentStatement(Token identifier, Token assignOperator, Expression value)
    {
        Identifier = identifier;
        AssignOperator = assignOperator;
        Value = value;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitAssignmentStatement(this);
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
public class InstructionStatement : Statement
{
    public FunctionExpression Function { get; }
    public InstructionStatement(FunctionExpression function)
    {
        Function = function;
    }
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitInstructionStatement(this);
    }
}