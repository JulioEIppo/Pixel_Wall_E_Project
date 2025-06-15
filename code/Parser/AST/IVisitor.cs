using System.Reflection.Emit;
using System.Text.RegularExpressions;

public interface IExpressionVisitor<T>
{
    T VisitBinaryExpression(BinaryExpression expr);
    T VisitUnaryExpression(UnaryExpression expr);
    T VisitLiteralExpression(LiteralExpression expr);
    T VisitGroupingExpression(GroupingExpression expr);
    T VisitVarExpression(VarExpression expr);
}

public interface IStatementVisitor<T>
{
    T VisitLabelStatement(LabelStatement stmt);
    T VisitGoToStatement(GoToStatement stmt);
    T VisitExpressionStatement(ExpressionStatement stmt);
    T VisitVarDeclaration(VarDeclaration varDeclaration);

}