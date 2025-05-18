public class LexicalAnalyzer
{
    public static readonly Dictionary<string, TokenType> methodsAndFunctions = new()
{
    {"Spawn",TokenType.Spawn},
    { "Color", TokenType.Color},
    { "Size",TokenType.Size},
    { "DrawLine",TokenType.DrawLine},
    { "DrawCircle",TokenType.DrawCircle},
    { "DrawRectangle",TokenType.DrawRectangle},
    { "Fill",TokenType.Fill},
    { "GoTo",TokenType.GoTo},
    { "GetActualX",TokenType.GetActualX},
    { "GetActualY",TokenType.GetActualY},
    { "GetCanvasSize",TokenType.GetCanvasSize},
    { "GetColorCount",TokenType.GetColorCount},
    { "IsBrushColor",TokenType.IsBrushColor},
    { "IsCanvasColor",TokenType.IsCanvasColor},
    };
    public static readonly Dictionary<string, TokenType> operators = new()
{
    { "<-",TokenType.Assign},
    { ",",TokenType.Comma},
    { ";",TokenType.Semicolon},
    { "[",TokenType.OpenBracket},
    { "]",TokenType.ClosedBracket},
    { "{",TokenType.OpenCurlyBraces},
    { "}",TokenType.ClosedCurlyBraces},
    {"+",TokenType.Addition},
    {"-",TokenType.Subtract},
    {"*",TokenType.Multiplication},
    {"/",TokenType.Division},
    {"**",TokenType.Power},
    {"%",TokenType.Module},
    {"&&",TokenType.And},
    {"||",TokenType.Or},
    {"==",TokenType.Equal},
    {">=",TokenType.BiggerOrEqual},
    {"<=>",TokenType.LesserOrEqual},
    {">",TokenType.Bigger},
    {"<",TokenType.Lesser},
};
    public static Dictionary<string, TokenType> symbols = new()
    ;
    public static Dictionary<string, TokenType> text = new();
}