public class Token
{
    public string? Lexeme { get; }
    public TokenType Type { get; }
    public int Line { get; }
    public override string ToString() => $"{Type}:{Lexeme} in {Line}";
    public Token(string lexeme, TokenType type, int line)
    {
        Lexeme = lexeme;
        Type = type;
        Line = line;
    }

}
public enum TokenType
{
    //Operators
    Addition, Subtract, Multiplication, Division, Power, Module, And, Or, Equal, BiggerOrEqual, LesserOrEqual, Bigger, Lesser,
    //Methods
    Spawn, Color, Size, DrawLine, DrawCircle, DrawRectangle, Fill, GoTo,
    //Functions
    GetActualX, GetActualY, GetCanvasSize, GetColorCount, IsBrushColor, IsCanvasColor,
    //Symbols
    Assign, Comma, Semicolon, OpenBracket, ClosedBracket, OpenCurlyBraces, ClosedCurlyBraces,

    Number, Identifier, Label,

    EndOfLine, EndOfFile
}