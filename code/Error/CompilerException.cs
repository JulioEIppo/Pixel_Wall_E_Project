public class CompilerException: Exception
{
     public int Line { get; }
    public string ErrorMessage { get; }

    public CompilerException(int line, string message)
    {
        Line = line;
        ErrorMessage = message;
    }
    public override string Message => $" Error at line {Line}: {ErrorMessage}";

}