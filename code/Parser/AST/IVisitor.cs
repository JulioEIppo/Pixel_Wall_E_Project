using System.Reflection.Emit;
using System.Text.RegularExpressions;

public interface IExpressionVisitor<T>
{
    T VisitBinaryExpression(BinaryExpression expr);
    T VisitUnaryExpression(UnaryExpression expr);
    T VisitLiteralExpression(LiteralExpression expr);
    T VisitGroupingExpression(GroupingExpression expr);
    T VisitIdentifierExpression(IdentifierExpression expr);
    T VisitFunctionExpression(FunctionExpression expr);
}

public interface IStatementVisitor<T>
{
    T VisitLabelStatement(LabelStatement stat);
    T VisitAssignmentStatement(AssignmentStatement stat);
    T VisitGoToStatement(GoToStatement stat);
    T VisitInstructionStatement(InstructionStatement stat);

}