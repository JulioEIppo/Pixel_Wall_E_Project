using System.Reflection.Emit;
using System.Text.RegularExpressions;
namespace PixeLWallE

{
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
        void Visit(LabelStatement stmt);
        void Visit(GoToStatement stmt);
        void Visit(ExpressionStatement stmt);
        void Visit(VarDeclaration stmt);
        void Visit(SpawnStmt stmt);
        void Visit(ColorStmt stmt);
        void Visit(SizeStmt stmt);
        void Visit(DrawLineStmt stmt);
        void Visit(DrawCircleStmt stmt);
        void Visit(DrawRectangleStmt stmt);
        void Visit(FillStmt stmt);

    }
}