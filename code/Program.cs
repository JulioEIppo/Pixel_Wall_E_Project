using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Example Pixel-Walle code (replace with your own test cases)
        string code = @"
             Spawn(0,0)
            n <- 4
            Color Red
            fooBar123
            123abc
            42
            true false
            == != <= >= < > + - * / ** %
            @ $ # ~ `
        ";

        // Create the lexer
        var lexer = new Lexer(code);

        // Lex the input and print tokens
        Console.WriteLine("Tokens:");
        foreach (var token in lexer.Lex())
        {
            Console.WriteLine(token);
        }

        // Print errors, if any
        if (lexer.Errors.Any())
        {
            Console.WriteLine("\nErrors:");
            foreach (var error in lexer.Errors)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            Console.WriteLine("\nNo syntax errors found.");
        }
    }
}