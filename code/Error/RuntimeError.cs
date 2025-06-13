public class RuntimeErrorException : CompilerException
{
    public RuntimeErrorException(Token token, string message) : base(token, message) { }

    public override string Message => base.Message;

}