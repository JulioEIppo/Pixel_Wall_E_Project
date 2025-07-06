using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.AccessControl;

namespace PixelWallE
{
    public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private WallEEngine wallEEngine;
        private readonly Dictionary<string, INativeFunction> nativeFunctions;
        private Environment environment = new();
        private Dictionary<string, INativeFunction> NativeFunctions = new();
        private LabelTable labelTable = new();
        private int currentLine;
        public Interpreter(WallEEngine engine)
        {
            wallEEngine = engine;
            nativeFunctions = new()
            {
                {"GetActualX", new GetActualXFunction(engine)},
                {"GetActualY", new GetActualYFunction(engine)},
                {"GetCanvasSize", new GetCanvasSizeFunction(engine)},
                {"GetColorCount", new GetColorCountFunction(engine)},
                {"IsBrushColor", new IsBrushColorFunction(engine)},
                {"IsBrushSize", new IsBrushSizeFunction(engine)},
                {"IsCanvasColor", new IsCanvasColorFunction(engine)}
            };
        }

        public void Interpret(List<Statement> statements)
        {
            try
            {
                FillLabels(statements);
                currentLine = 0;
                while (currentLine < statements.Count)
                {
                    int aux = currentLine;
                    RunStatement(statements[currentLine]);
                    if (aux == currentLine) currentLine++;
                }
            }
            catch (RuntimeErrorException err)
            {
                WallE.RuntimeError(err);
            }
        }
        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private void RunStatement(Statement statement)
        {
            statement.Accept(this);
        }

        //EXPRESSIONS
        public object Visit(BinaryExpression expression)
        {
            object left = Evaluate(expression.Left);
            object right = Evaluate(expression.Right);
            switch (expression.Operator.Type)
            {
                case TokenType.Addition:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left + (int)right;

                case TokenType.Subtract:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left - (int)right;

                case TokenType.Multiplication:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left * (int)right;

                case TokenType.Division:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left / (int)right;

                case TokenType.Power:
                    CheckNumber(left, right, expression.Operator);
                    return HandlePower((int)left, (int)right, expression.Operator);

                case TokenType.Modulo:
                    CheckNumber(left, right, expression.Operator);
                    return HandleModulo((int)left, (int)right, expression.Operator);

                case TokenType.And:
                    var (aBool1, aBool2) = CheckBooleanOperands(left, right, expression.Operator);
                    return aBool1 && aBool2;

                case TokenType.Or:
                    var (oBool1, oBool2) = CheckBooleanOperands(left, right, expression.Operator);
                    return oBool1 || oBool2;

                case TokenType.Equal:
                    return IsEqual(left, right);

                case TokenType.NotEqual:
                    return !IsEqual(left, right);

                case TokenType.BiggerOrEqual:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left >= (int)right;

                case TokenType.Bigger:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left > (int)right;

                case TokenType.LesserOrEqual:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left <= (int)right;

                case TokenType.Lesser:
                    CheckNumber(left, right, expression.Operator);
                    return (int)left < (int)right;

                default: throw new RuntimeErrorException(expression.Operator, "Unknown operator");
            }
        }
        public object Visit(CallExpression expr)
        {
            if (!NativeFunctions.TryGetValue(expr.Function.Value, out var function))
                throw new RuntimeErrorException(expr.Function, $"Function: {expr.Function.Value}  not found");
            if (expr.Arguments.Count != function.Arity)
            {
                throw new RuntimeErrorException(expr.Function, $"Invalid amount of args, required arguments: {function.Arity}");
            }
            List<object> args = new();
            foreach (var arg in expr.Arguments)
            {
                args.Add(arg);
            }
            return function.Invoke(args, expr.Function);
        }
        public object Visit(VarExpression var)
        {
            return environment.Get(var.Token);
        }

        public object Visit(LiteralExpression expression)
        {
            return expression.Value;
        }
        public object Visit(GroupingExpression expression)
        {
            return Evaluate(expression);
        }
        public object Visit(UnaryExpression expression)
        {
            object right = Evaluate(expression.Expression);
            switch (expression.Operator.Type)
            {
                case TokenType.Subtract:
                    if (right is int rInt)
                    {
                        return -rInt;
                    }
                    throw new RuntimeErrorException(expression.Operator, "Integer operand required");
                case TokenType.Not:
                    if (right is bool rBool)
                    {
                        return !rBool;
                    }
                    throw new RuntimeErrorException(expression.Operator, "Boolean operand required");
                default: throw new RuntimeErrorException(expression.Operator, "Unknown operator");
            }
        }


        //STATEMENTS
        public void Visit(SpawnStmt spawn)
        {
            object x = Evaluate(spawn.X);
            object y = Evaluate(spawn.Y);
            if (x is not int  && y is not int )
            {
            throw new RuntimeErrorException(spawn.Keyword, $"Expected int parameters for Spawn in line:{spawn.Keyword.Line}");
            }
                wallEEngine.Spawn((int)x, (int)y, spawn.Keyword);
        }
        public void Visit(ColorStmt colorStmt)
        {
            object colorString = Evaluate(colorStmt.Color);
            if (colorString is not string color)
            {
                throw new RuntimeErrorException(colorStmt.Keyword, $"Expected color string parameter in line{colorStmt.Keyword.Line}");
            }
            wallEEngine.SetColor(color, colorStmt.Keyword);
        }
        public void Visit(SizeStmt size)
        {
            object value = Evaluate(size.Size);
            if (value is not int sizeValue)
            {
                throw new RuntimeErrorException(size.Keyword, $"Expected int parameter for Size in line:{size.Keyword.Line}");
            }
            wallEEngine.SetSize(sizeValue, size.Keyword);
        }
        public void Visit(DrawLineStmt drawLine)
        {
            object dirX = Evaluate(drawLine.DirX);
            object dirY = Evaluate(drawLine.DirY);
            object distance = Evaluate(drawLine.Distance);
            if (dirX is not int || dirY is not int || distance is not int)
            {
                throw new RuntimeErrorException(drawLine.Keyword, $"Expected int parameters for DrawLine in line:{drawLine.Keyword.Line} ");
            }
            wallEEngine.DrawLine((int)dirX, (int)dirY, (int)distance, drawLine.Keyword);
        }
        public void Visit(DrawCircleStmt drawCircle)
        {
            object dirX = Evaluate(drawCircle.DirX);
            object dirY = Evaluate(drawCircle.DirY);
            object radius = Evaluate(drawCircle.Radius);
            if (dirX is not int || dirY is not int || radius is not int)
            {
                throw new RuntimeErrorException(drawCircle.Keyword, $"Expected int parameters for DrawCircle in line:{drawCircle.Keyword.Line}");
            }
            wallEEngine.DrawCircle((int)dirX, (int)dirY, (int)radius, drawCircle.Keyword);
        }
        public void Visit(DrawRectangleStmt drawRectangle)
        {
            object dirX = Evaluate(drawRectangle.DirX);
            object dirY = Evaluate(drawRectangle.DirY);
            object distance = Evaluate(drawRectangle.Distance);
            object width = Evaluate(drawRectangle.Width);
            object height = Evaluate(drawRectangle.Height);
            if (dirX is not int || dirY is not int || distance is not int || width is not int || height is not int)
            {
                throw new RuntimeErrorException(drawRectangle.Keyword, $"Expected int parameters for DrawRectangle in line:{drawRectangle.Keyword.Line}");
            }
            wallEEngine.DrawRectangle((int)dirX, (int)dirY, (int)distance, (int)width, (int)height, drawRectangle.Keyword);
        }
        public void Visit(FillStmt fill)
        {
            wallEEngine.Fill();
        }
        public void Visit(VarDeclaration var)
        {
            object value = Evaluate(var.Expression);
            environment.Define(var.ID, value);
        }
        public void Visit(LabelStatement label) { } // labels don't need to be evaluated 
        public void Visit(ExpressionStatement statement)
        {
            Evaluate(statement.Expression);
        }
        public void Visit(GoToStatement goToStatement)
        {
            var condition = Evaluate(goToStatement.Condition);
            if (condition is not bool)
            {
                throw new RuntimeErrorException(goToStatement.Keyword, $"Invalid condition, expected bool for GoTo in line:{goToStatement.Keyword.Line}");
            }
            if ((bool)condition)
            {
                currentLine = labelTable.GetLine(goToStatement.Keyword, goToStatement.Label);
            }
        }


        //HELPERS
        private void FillLabels(List<Statement> statements)
        {
            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i] is LabelStatement labelStatement)
                {
                    labelTable.Add(labelStatement.LabelToken);
                }
            }
        }
        private int HandlePower(int baseValue, int exponent, Token opToken)
        {
            if (exponent < 0)
            {
                throw new RuntimeErrorException(opToken, "Negative exponents are not supported");
            }
            if (baseValue == 0 && exponent == 0)
            {
                throw new RuntimeErrorException(opToken, "0^0 is not defined");
            }
            int result = 1;
            for (int i = 0; i < exponent; i++)
            {
                checked
                {
                    result *= baseValue;
                }
            }
            return result;
        }
        private void CheckNumber(object a, object b, Token opToken)
        {
            if (a is int && b is int) return;
            throw new RuntimeErrorException(opToken, "Operation requires integer operands");
        }
        private int HandleModulo(int a, int b, Token opToken)
        {
            if (b == 0)
            {
                throw new RuntimeErrorException(opToken, "Module by zero is not allowed");
            }
            return a % b;
        }
        private bool IsEqual(object a, object b)
        {
            return Equals(a, b);
        }
        private (bool, bool) CheckBooleanOperands(object a, object b, Token opToken)
        {
            if (a is bool lBool && b is bool rBool) return (lBool, rBool);
            throw new RuntimeErrorException(opToken, "Operation requires boolean operands");
        }

    }
}