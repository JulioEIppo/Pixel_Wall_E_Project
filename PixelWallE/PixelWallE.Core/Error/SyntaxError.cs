using System;

namespace PixeLWallE
{
    public class SyntaxErrorException : Exception
    {
        public int Line { get; }
        public string ErrorMessage { get; set; }
        public SyntaxErrorException(int line, string errorMessage)
        {
            Line = line;
            ErrorMessage = errorMessage;
        }
        public override string Message => $"Syntax error at line {Line}: {ErrorMessage}";
    }
}