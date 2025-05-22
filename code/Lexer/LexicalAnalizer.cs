using System.Text.RegularExpressions;

public class LexicalAnalyzer
{
    public static readonly Dictionary<string, TokenType> keywords = new(StringComparer.OrdinalIgnoreCase)
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
   {"<-",TokenType.Assign},
   {"<=",TokenType.LesserOrEqual},
   {">=",TokenType.BiggerOrEqual},
   {"!=",TokenType.NotEqual},
   { "==",TokenType.Equal},
   {"&&",TokenType.And},
   {"||",TokenType.Or},
   {"!",TokenType.Not},
   { "+",TokenType.Addition},
   {"-",TokenType.Subtract},
   {"*", TokenType.Multiplication},
   {"/", TokenType.Division},
   {"**", TokenType.Power},
   {"%",TokenType.Module},
   {"<",TokenType.Lesser},
   {">",TokenType.Bigger},
   {",",TokenType.Comma},
   {"(", TokenType.OpenBracket},
   {")", TokenType.ClosedBracket},
   {"[",TokenType.OpenSquareBracket},
   {"]",TokenType.ClosedSquareBracket},
};
    public static readonly Regex operatorRegex = new Regex(@"^(<-|<=|>=|<|>|==|\*\*|&&|\|\||[+\-*/%><,()\[\]])");


    public static readonly List<(Regex, TokenType)> TokenRegex = new()
   {
    (new Regex(@"^[\s\t]+"), TokenType.Spaces),

    (new Regex(@"^\n"), TokenType.EndOfLine),

    (new Regex(@"^-?\d+"),TokenType.Number),

    (new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*"), TokenType.Identifier),

    };

}