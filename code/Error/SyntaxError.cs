public class SyntaxErrorException : CompilerException
{

    public SyntaxErrorException(int line, string message) : base(line, message) { }
    public override string Message => $"Syntax error at line {Line}: {ErrorMessage}";
}
