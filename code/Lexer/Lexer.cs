using System.Text.RegularExpressions;

public class Lexer
{
    private string Input { get; }
    private int Position { get; set; }
    private int CurrentLine { get; set; }
    public List<SyntaxErrorException> Errors { get; } = new List<SyntaxErrorException>();
    public Lexer(string input, int position)
    {
        Input = input;
        Position = position;
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
    private char Peek()
    {
        int nextPosition = Position + 1;
        if (nextPosition >= Input.Length)
        {
            return '\0';
        }
        return Input[nextPosition];
    }

    private void Consume() => Position++;
    private bool IsAtEnd() => Position >= Input.Length;


    public IEnumerable<Token> Lex(string input)
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
        var opMatch = LexicalAnalyzer.operatorRegex.Match(Input.Substring(Position));
        if (opMatch.Success)
        {
            string op = opMatch.Value;
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
                Position += value.Length;
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
        Errors.Add(new SyntaxErrorException(CurrentLine, Position, $"Unexpected character: {GetActualChar}"));
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
            Errors.Add(new SyntaxErrorException(CurrentLine, Position, $"Invalid number format: {value}"));
            return null;
        }
        if (type == TokenType.Boolean)
        {
            if (bool.TryParse(value, out bool parsedBool))
            {
                parsedValue = parsedBool;
                return new Token(type, value, CurrentLine, parsedValue);
            }
            else
            {
                Errors.Add(new SyntaxErrorException(CurrentLine, Position, $"Invalid boolean format: {value}"));
                return null;
            }
        }
        Errors.Add(new SyntaxErrorException(CurrentLine, Position, $"Unexpected token: {value}"));
        Position += value.Length;
        return null;
    }

}

