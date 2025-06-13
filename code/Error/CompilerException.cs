public class CompilerException: Exception
{
    public Token Token { get; } // using a token for more information
    public string ErrorMessage { get; }

    public CompilerException(Token token, string message)
    {
        Token = token;
        ErrorMessage = message;
    }
    public override string Message => $" Error at line {Token.Line}: {ErrorMessage}";

}