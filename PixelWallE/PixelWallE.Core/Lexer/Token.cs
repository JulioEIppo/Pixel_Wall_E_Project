namespace PixelWallE
{
    public class Token
    {
        public string Value { get; }
        public TokenType Type { get; }
        public int Line { get; }
        public object? ParsedValue { get; }
        // public override string ToString() => $"{Type} Token:{Value} ,in line {Line}";
        public Token(TokenType type, string value, int line, object? parsedValue = null)
        {
            Value = value;
            Type = type;
            Line = line;
            ParsedValue = parsedValue;
        }

    }
    public enum TokenType
    {
        //Operators
        Addition, Subtract, Multiplication, Division, Power, Modulo, And, Or, Not, Equal, NotEqual, BiggerOrEqual, LesserOrEqual, Bigger, Lesser,
        //Methods
        Spawn, Color, Size, DrawLine, DrawCircle, DrawRectangle, Fill, GoTo,
        //Functions
        GetActualX, GetActualY, GetCanvasSize, GetColorCount, IsBrushColor, IsBrushSize, IsCanvasColor,
        //Symbols
        Assign, Comma, OpenBracket, ClosedBracket, OpenSquareBracket, ClosedSquareBracket,

        Boolean,Number,InvalidNumber, Identifier, Label, String,

        EndOfLine, EndOfFile, Spaces
    }
}