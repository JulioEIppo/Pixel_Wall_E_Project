using System.Reflection.Emit;
using System.Text.RegularExpressions;

public interface IExpressionVisitor<T>
{
    T Visit(BinaryExpression expr);
    T Visit(UnaryExpression expr);
    T Visit(LiteralExpression expr);
    T Visit(GroupingExpression expr);
    T Visit(VarExpression expr);
    T Visit(CallExpression expr);
}

public interface IStatementVisitor<T>
{
    T Visit(LabelStatement stmt);
    T Visit(GoToStatement stmt);
    T Visit(ExpressionStatement stmt);
    T Visit(VarDeclaration stmt);
    T Visit(SpawnStmt stmt);
    T Visit(ColorStmt stmt);
    T Visit(SizeStmt stmt);
    T Visit(DrawLineStmt stmt);
    T Visit(DrawCircleStmt stmt);
    T Visit(DrawRectangleStmt stmt);
    T Visit(FillStmt stmt);

}