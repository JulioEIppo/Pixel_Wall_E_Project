public class Token
{
    public string Value { get; private set; }
    public TokenType Type { get; private set; }
    public int Line { get; private set; }
    public Token(TokenType type, string value, int line)
    {
        Value = value;
        Type = type;
        Line = line;
    }
    public override string ToString() => $"{Type}:{Value} in {Line}";

}
public enum TokenType
{
    Unknown,
    Number,
    Text,
    Keyword,
    Identifier,
    Symbol,
}
public static class TokenValues
{
    //Operators
    public const string Add = "Addition"; // +
    public const string Sub = "Subtract"; // -
    public const string Mul = "Multiplication"; // *
    public const string Div = "Division"; // /
    public const string Pow = "Power"; // **
    public const string Mod = "Module"; // %
    public const string And = "And"; // &&
    public const string Or = "Or"; // ||
    public const string Equal = "Equal"; // ==
    public const string BiggerOrEqual = "BiggerOrEqual"; // >=
    public const string LesserOrEqual = "LesserOrEqual"; // <=
    public const string Bigger = "Bigger"; // >
    public const string Lesser = "Lesser"; // <

    //Identifiers
    public const string Label = "Label";

    //Methods
    public const string Spawn = "Spawn";
    public const string Color = "Color";
    public const string Size = "Size";
    public const string DrawLine = "DrawLine";
    public const string DrawCircle = "DrawCircle";
    public const string DrawRectangle = "DrawRectangle";
    public const string Fill = "Fill";
    public const string GoTo = "GoTo";

    //Functions
    public const string GetActualX = "GetActualX";
    public const string GetActualY = "GetActualY";
    public const string GetCanvasSize = "GetCanvasSize";
    public const string GetColorCount = "GetColorCount";
    public const string IsBrushColor = "IsBrushColor";
    public const string IsCanvasColor = "IsCanvasColor";

    //Symbols
    public const string Assign = "Assign"; // <-
    public const string ExpressionSeparator = "ExpressionSeparator"; // ,
    public const string StatementSeparator = "StatementSeparator"; // ;
    public const string OpenBracket = "OpenBracket"; // (
    public const string ClosedBracket = "CloseBracket"; // )
    public const string OpenCurlyBraces = "OpenCurlyBraces"; // {
    public const string ClosedCurlyBraces = "CloseCurlyBraces"; // }

}