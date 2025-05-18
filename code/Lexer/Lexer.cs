using System.Text.RegularExpressions;

public class Lexer
{
    private Regex numbers = new Regex(@"^-?\d+");
    private Regex identifiers = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_-]*");
    private Regex symbols = new Regex(@"^(<-|<=|>=|<|>|==|\*\*|&&|\|\||[+\-*/%><,()\[\]])");
    private Regex spaces = new Regex(@"^[\s\t+]");
    private Regex invalid = new Regex(@"^[=,:{}\\]");
    public Dictionary<string, TokenType>? Dictionary { get; private set; }
    public List<Token> Scan(string entry)
    {
        List<Token> tokens = new();
        string[] lines = entry.Split('\n');
        int currentLine = 0;

        foreach (string line in lines)
        {
            int position = 0;
            while (position < lines.Length)
            {
                char currentChar = line[position];
                //check for identifiers or labels
                var iD = identifiers.Match(line.Substring(position));
                if (iD.Success)
                {
                    string value = iD.Value;
                    tokens.Add(new Token(value, TokenType.Identifier, currentLine));
                    position += iD.Length;
                }
                else
                {
                    //check if there's a label
                }
                //check for numbers
                var numMatch = numbers.Match(line.Substring(position));
                if (numMatch.Success)
                {
                    string value = numMatch.Value;
                    tokens.Add(new Token(value, TokenType.Number, currentLine));
                    position += numMatch.Length;
                }
                else
                {
                    //check here
                }

                // var symMatch = symbols.Match(line.Substring(position));
                // if (true)
                // {
                    
                // }
            }
        }

        return tokens;
    }







}