using System.Reflection.Emit;
using System.Text.RegularExpressions;

public class Parser
{
    public TokenStream Stream { get; }
    public List<Exception> Errors { get; set; }
    public Parser(TokenStream tokens, List<Exception> errors)
    {
        Stream = tokens;
        Errors = errors;
    }
    public void ReportSyntaxError(string message)
    {
        Errors.Add(new SyntaxErrorException(Stream.Current!.Line, message));
    }
    public List<Statement> Parse()
    {
        List<Statement> statements = new();
        while (!Stream.IsAtEnd())
        {
            try
            {
                if (Stream.Current.Type == TokenType.Identifier && Stream.Peek().Type == TokenType.EndOfLine)
                {
                    statements.Add(ParseLabel());
                }
                statements.Add(ParseDeclaration());
            }
            catch (SyntaxErrorException error)
            {
                Errors.Add(error);
                Synchronize();
            }
        }
        return statements;
    }
    private Statement ParseLabel()
    {
        if (Stream.Current.Type == TokenType.Identifier && Stream.Peek().Type == TokenType.EndOfLine)
        {
            string label = Stream.Current.Value;
            Stream.Match(TokenType.Identifier);
            int line = Stream.Current.Line;
            Stream.Match(TokenType.EndOfLine);
            return new LabelStatement(label, line);
        }
        throw new SyntaxErrorException(Stream.Current.Line, "Invalid Label");
    }
    private Statement ParseDeclaration()
    {
        if (Stream.Peek().Type == TokenType.Identifier && Stream.Peek(2).Type == TokenType.Assign)
            return ParseVarDeclaration();
        return ParseExpressionStatement();
    }
    private Statement ParseVarDeclaration()
    {
        Token id = Stream.Next();
        Stream.Next();    //skip assignment operator
        Expression expr = ParseExpression();
        return new VarDeclaration(id.Value, expr);
    }
    private Statement ParseExpressionStatement()
    {
        Expression expr = ParseExpression();
        Stream.Match(TokenType.EndOfLine);
        return new ExpressionStatement(expr);
    }
    private Expression ParseExpression()
    {
        return ParseOr();
    }
    private Expression ParseOr()
    {
        Expression expr = ParseAnd();
        while (Stream.Match(TokenType.Or))
        {
            Token op = Stream.Previous();
            Expression right = ParseAnd();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }
    private Expression ParseAnd()
    {
        Expression expr = ParseEquality();
        while (Stream.Match(TokenType.And))
        {
            Token op = Stream.Previous();
            Expression right = ParseComparison();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }
    private Expression ParseEquality()
    {
        Expression expr = ParseComparison();
        while (Stream.Match(TokenType.Equal, TokenType.NotEqual))
        {
            Token op = Stream.Previous();
            Expression right = ParseOr();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }
    private Expression ParseComparison()
    {
        Expression expr = ParseAddition();
        while (Stream.Match(TokenType.BiggerOrEqual, TokenType.Bigger, TokenType.LesserOrEqual, TokenType.Lesser))
        {
            Token? op = Stream.Previous();
            Expression right = ParseAddition();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }
    private Expression ParseAddition()
    {
        Expression expr = ParseMultiplication();
        while (Stream.Match(TokenType.Addition, TokenType.Subtract))
        {
            Token op = Stream.Previous();
            Expression right = ParseMultiplication();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }
    private Expression ParseMultiplication()
    {
        Expression expr = ParseUnary();
        while (Stream.Match(TokenType.Multiplication, TokenType.Division, TokenType.Modulo, TokenType.Power))
        {
            Token op = Stream.Previous();
            Expression right = ParseUnary();
            expr = new BinaryExpression(expr, op, right);
        }
        return expr;
    }
    private Expression ParseUnary()
    {
        if (Stream.Match(TokenType.Subtract, TokenType.Not))
        {
            Token op = Stream.Previous();
            Expression right = ParseUnary();
            return new UnaryExpression(op, right);
        }
        return ParsePrimary();
    }
    private Expression ParsePrimary()
    {
        if (Stream.Match(TokenType.Number))
        {
            return new LiteralExpression(Stream.Previous());
        }
        if (Stream.Match(TokenType.Identifier))
        {
            return new VarExpression(Stream.Previous());
        }
        if (Stream.Match(TokenType.OpenBracket))
        {
            var expr = ParseExpression();
            if (!Stream.Match(TokenType.ClosedBracket))
            {
                throw new SyntaxErrorException(Stream.Previous().Line, "Expected )");
            }
            return new GroupingExpression(expr);
        }
        throw new SyntaxErrorException(Stream.Current.Line, "Expected expression");
    }
    private void Synchronize()
    {
        while (!Stream.IsAtEnd())
        {
            if (Stream.Current.Type == TokenType.EndOfLine)
            {
                Stream.Next();
                break;
            }
            Stream.Next();
        }
    }
}