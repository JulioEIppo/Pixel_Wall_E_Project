using System.Reflection.Emit;

public interface IVisitor<T>
{
    T VisitBinaryExpression(BinaryExpression expr);
    T VisitUnaryExpression(UnaryExpression expr);
    T VisitLiteralExpression(LiteralExpression expr);
    T VisitIdentifierExpression(IdentifierExpression expr);
    T VisitFunctionExpression(FunctionExpression expr);
    T VisitLabelStatement(LabelStatement stat);
    T VisitAssignmentStatement(AssignmentStatement stat);
    T VisitGoToStatement(GoToStatement stat);
    T VisitInstructionStatement(InstructionStatement stat);
}