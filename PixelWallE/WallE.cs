using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DynamicData.Binding;
using DynamicData.Kernel;

namespace PixelWallE
{
    public static class WallE
    {
        private static WallEEngine engine = new WallEEngine(100);
        private static Interpreter interpreter = new Interpreter(engine);
        public static List<string> Errors = new();
        private static string errorMessage = "";
        public static bool HadSyntaxError { get; set; }
        public static bool HadRuntimeError { get; set; }
        private static void RunFile(string input)
        {
            string source = File.ReadAllText(input, Encoding.Default);
            RunCode(source);
            if (HadSyntaxError) System.Environment.Exit(30); //syntax error exit 
            if (HadRuntimeError) System.Environment.Exit(50); //runtime error exit
        }


        public static void RunCode(string source)
        {
            HadSyntaxError = false;
            HadRuntimeError = false;
            Run(source);
        }
        private static void Run(string source)
        {
            Lexer lexer = new Lexer(source);
            Parser parser = new Parser(new TokenStream(lexer.Lex()));
            List<Statement> statements = parser.Parse();
            if (HadSyntaxError) return;
            interpreter.Interpret(statements);
        }
        public static void SetCanvasSize(int size)
        {
            engine = new WallEEngine(size);
            interpreter = new Interpreter(engine);
        }
        public static CanvasColor[,] GetCanvas()
        {
            return engine.GetCanvas();
        }
        public static void SyntaxError(SyntaxErrorException error)
        {
            ReportSyntaxError(error.Message);
        }
        public static void ReportSyntaxError(string message)
        {
            errorMessage = $"{message}";
            Errors.Add(errorMessage);
            HadSyntaxError = true;
        }
        public static void RuntimeError(RuntimeErrorException error)
        {
            Console.WriteLine(error.ErrorMessage);
            errorMessage = $"{error.ErrorMessage}";
            Errors.Add(errorMessage);
            HadRuntimeError = true;
        }

    }
}