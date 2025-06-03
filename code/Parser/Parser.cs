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
        int line = Stream.Current?.Line ?? -1;
        Errors.Add(new SyntaxErrorException(line, message));
    }
    private Token? Consume(TokenType type, string errorMessage)
    {
        if (Stream.Match(type))
        {
            return Stream.Previous();
        }
        ReportSyntaxError(errorMessage);
        return null;
    }

    private Expression? ParseExpression()
    {
        return ParseEquality();
    }

    private Expression? ParseEquality()
    {
        Expression expr = ParseComparison()!;
        while (Stream.Match(TokenType.Equal, TokenType.NotEqual))
        {
            Token? op = Stream.Previous();
            Expression right = ParseComparison()!;
            expr = new BinaryExpression(expr, op!, right);
        }
        return expr;
    }
    private Expression? ParseComparison()
    {
        Expression expr = ParseAddition()!;
        while (Stream.Match(TokenType.BiggerOrEqual, TokenType.Bigger, TokenType.LesserOrEqual, TokenType.Lesser))
        {
            Token? op = Stream.Previous();
            Expression right = ParseAddition()!;
            expr = new BinaryExpression(expr, op!, right);
        }
        return expr;
    }
    private Expression? ParseAddition()
    {
        Expression expr = ParseMultiplication()!;
        while (Stream.Match(TokenType.Addition, TokenType.Subtract))
        {
            Token? op = Stream.Previous();
            Expression right = ParseMultiplication()!;
            expr = new BinaryExpression(expr, op!, right);
        }
        return expr;
    }
    private Expression? ParseMultiplication()
    {
        Expression expr = ParseUnary()!;
        while (Stream.Match(TokenType.Multiplication, TokenType.Division, TokenType.Module, TokenType.Power))
        {
            Token? op = Stream.Previous()!;
            Expression right = ParseUnary()!;
            expr = new BinaryExpression(expr, op!, right);
        }
        return expr;
    }
    private Expression? ParseUnary()
    {
        if (Stream.Match(TokenType.Subtract, TokenType.Not))
        {
            Token? op = Stream.Previous();
            Expression right = ParseUnary()!;
            return new UnaryExpression(op!, right); 
        }
        return ParsePrimary();
    }
    private Expression? ParsePrimary()
    {
        if (Stream.Match(TokenType.Number))
        {
            return new LiteralExpression(Stream.Previous()!.ParsedValue!);
        }
        if (Stream.Match(TokenType.Identifier))
        {
            return new IdentifierExpression(Stream.Previous()!.Value);
        }
        if (Stream.Match(TokenType.OpenBracket))
        {
            var expr = ParseExpression();
            if (Consume(TokenType.ClosedBracket, "Expected ')'") == null)
            {
                return null;
            }
            return expr;
        }
        return null;
    }
}