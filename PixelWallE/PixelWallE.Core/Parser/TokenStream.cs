using System.Collections.Generic;
using System.Linq;

namespace PixeLWallE
{
    public class TokenStream
    {
        private readonly List<Token> Tokens;
        public int Position = 0;
        public Token Current => CurrentToken();
        public TokenStream(IEnumerable<Token> tokens)
        {
            Tokens = tokens.ToList();
        }
        private Token EndOfFileToken()
        {
            int line = Tokens.Count > 0 ? Tokens[^1].Line : -1;
            return new Token(TokenType.EndOfFile, "", line);
        }
        public Token Next()
        {
            if (Position < Tokens.Count())
            {
                return Tokens[Position++];
            }
            return EndOfFileToken();
        }
        public Token Peek(int n = 1)
        {
            int index = Position + n - 1;
            if (index < Tokens.Count)
            {
                return Tokens[index];
            }
            return EndOfFileToken();

        }
        public bool IsAtEnd()
        {
            return Position >= Tokens.Count;
        }
        public Token CurrentToken()
        {
            if (Position < Tokens.Count)
            {
                return Tokens[Position];
            }
            return EndOfFileToken();

        }
        public bool Match(params TokenType[] types)
        {
            foreach (var type in types)
                if (!IsAtEnd() && CurrentToken()?.Type == type)
                {
                    Next();
                    return true;
                }
            return false;
        }
        public Token Previous()
        {
            if (Position > 0 && Position - 1 < Tokens.Count)
            {
                return Tokens[Position - 1];
            }
            return EndOfFileToken();

        }
    }
}