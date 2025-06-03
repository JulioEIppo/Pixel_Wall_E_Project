using System.Text.RegularExpressions;

public class Lexer
{
    private string Input { get; }
    private int Position { get; set; }
    private int CurrentLine { get; set; }
    public List<SyntaxErrorException> Errors { get; } = new List<SyntaxErrorException>();
    public Lexer(string input)
    {
        Input = input;
        Position = 0;
        CurrentLine = 1;
    }
    private char GetActualChar
    {
        get
        {
            if (Position >= Input.Length)
            {
                return '\0';
            }
            return Input[Position];
        }
    }
    private bool IsAtEnd() => Position >= Input.Length;


    public IEnumerable<Token> Lex()
    {
        while (!IsAtEnd())
        {
            var token = GetNextToken();
            if (token != null && token.Type != TokenType.Spaces)
            {
                yield return token;
            }
        }
        yield return new Token(TokenType.EndOfFile, "EOF", CurrentLine);
    }

    private Token? GetNextToken()
    {
        // Console.WriteLine($"GetNextToken at position {Position}: '{Input.Substring(Position)}'");
        var opMatch = LexicalAnalyzer.operatorRegex.Match(Input.Substring(Position));
        if (opMatch.Success)
        {
            string op = opMatch.Value;
            //  Console.WriteLine($"Matched operator: '{op}' at position {Position}");
            if (LexicalAnalyzer.operators.TryGetValue(op, out TokenType type))
            {
                Position += op.Length;
                return new Token(type, op, CurrentLine);
            }
        }
        foreach (var (regex, type) in LexicalAnalyzer.TokenRegex)
        {
            var match = regex.Match(Input.Substring(Position));
            if (match.Success)
            {
                string value = match.Value;
                //  Console.WriteLine($"Matched {type}: '{value}' at position {Position}");
                Position += value.Length;
                if (type == TokenType.Spaces)
                {
                    return null;
                }
                if (type == TokenType.EndOfLine)
                {
                    CurrentLine++;
                    return null;
                }
                if (type == TokenType.Identifier && LexicalAnalyzer.keywords.TryGetValue(value, out TokenType keywordType))
                {
                    return new Token(keywordType, value, CurrentLine);
                }
                return TokenCreator(type, value);
            }
        }
        Errors.Add(new SyntaxErrorException(CurrentLine, $"Unexpected character: {GetActualChar}"));
        Position++;
        return null;
    }

    private Token? TokenCreator(TokenType type, string value)
    {
        object? parsedValue = null;
        if (type == TokenType.Number)
        {
            if (int.TryParse(value, out int parsedInt))
            {
                parsedValue = parsedInt;
                return new Token(type, value, CurrentLine, parsedValue);
            }
            Errors.Add(new SyntaxErrorException(CurrentLine, $"Invalid number format: {value}"));
            return null;
        }
        if (type == TokenType.Identifier)
        {
            return new Token(type, value, CurrentLine);
        }
        Errors.Add(new SyntaxErrorException(CurrentLine, $"Unexpected token: {value}"));
        return null;
    }

}

