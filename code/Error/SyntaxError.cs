public class SyntaxErrorException : Exception
{
    public int Line { get; }
    public int Position { get; }
    public string ErrorMessage { get; }

    public SyntaxErrorException(int line, int position, string message)
    {
        Line = line;
        Position = position;
        ErrorMessage = message;
    }
}
