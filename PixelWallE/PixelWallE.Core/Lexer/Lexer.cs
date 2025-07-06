using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
namespace PixelWallE
{
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
                    if (type == TokenType.Spaces)
                    {
                        return null;
                    }
                    if (type == TokenType.EndOfLine)
                    {
                        CurrentLine++;
                        Console.WriteLine(type);
                        return new Token(TokenType.EndOfLine, "", CurrentLine);
                    }
                    if (type == TokenType.Identifier && LexicalAnalyzer.keywords.TryGetValue(value, out TokenType keywordType))
                    {
                        object parsedBool = keywordType == TokenType.Boolean ? bool.Parse(value) : null!;
                        return new Token(keywordType, value, CurrentLine, parsedBool);
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
            if (type == TokenType.Number)
            {
                if (int.TryParse(value, out int parsedInt))
                {
                    return new Token(type, value, CurrentLine, parsedInt);
                }
                Errors.Add(new SyntaxErrorException(CurrentLine, $"Invalid number format: {value}"));
                return null;
            }
            if (type == TokenType.Boolean)
            {
                if (bool.TryParse(value, out bool parsedBool))
                {
                    return new Token(type, value, CurrentLine, parsedBool);
                }
                Errors.Add(new SyntaxErrorException(CurrentLine, $"Invalid format, expected bool:{value}"));
                return null;
            }
            if (type == TokenType.String)
            {
                value = value.Substring(1, value.Length - 2); // remove quotes
                return new Token(type, value, CurrentLine, value);
            }
            if (type == TokenType.Identifier)
            {
                return new Token(type, value, CurrentLine);
            }
            Errors.Add(new SyntaxErrorException(CurrentLine, $"Unexpected token: {value}"));
            return null;
        }
    }
}