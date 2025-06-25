using System;

namespace PixeLWallE
{public class RuntimeErrorException : Exception
{
    public Token Token { get; }
    public string ErrorMessage { get; set; }
    public RuntimeErrorException(Token token, string errorMessage)
    {
        Token = token;
        ErrorMessage = errorMessage;
    }

    public override string Message => $"Error at Line:{Token?.Line}: {ErrorMessage}";

}}