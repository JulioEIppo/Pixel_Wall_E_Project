public class SyntaxErrorException : CompilerException
{
    public SyntaxErrorException(Token token, string message) : base(token, message) { }
    public override string Message => $"Syntax error at line {Token.Line}: {ErrorMessage}";
}
