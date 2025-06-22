using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
namespace PixeLWallE
{
    public class Parser
    {
        public TokenStream Stream { get; }
        public List<Exception> Errors { get; set; }
        public Parser(TokenStream tokens, List<Exception> errors)
        {
            Stream = tokens;
            Errors = errors;
        }
        public void ReportSyntaxError(string message)
        {
            Errors.Add(new SyntaxErrorException(Stream.Current!.Line, message));
        }
        public Token Consume(TokenType type, string message)
        {
            if (Stream.Current.Type == type)
            {
                return Stream.Next();
            }
            throw new SyntaxErrorException(Stream.Current.Line, message);
        }
        public List<Statement> Parse()
        {
            List<Statement> statements = new();
            while (!Stream.IsAtEnd())
            {
                try
                {
                    if (Stream.Current.Type == TokenType.Identifier && Stream.Peek().Type == TokenType.EndOfLine)
                    {
                        statements.Add(ParseLabel());
                    }
                    if (Stream.Current.Type == TokenType.Identifier && Stream.Peek().Type == TokenType.Assign)
                    {
                        statements.Add(ParseDeclaration());
                    }

                    statements.Add(ParseStatement());
                }
                catch (SyntaxErrorException error)
                {
                    Errors.Add(error);
                    Synchronize();
                }
            }
            return statements;
        }
        private Statement ParseStatement()
        {
            if (Stream.Match(TokenType.Spawn)) return ParseSpawn();
            if (Stream.Match(TokenType.Color)) return ParseColor();
            if (Stream.Match(TokenType.Size)) return ParseSize();
            if (Stream.Match(TokenType.DrawLine)) return ParseDrawLine();
            if (Stream.Match(TokenType.DrawCircle)) return ParseDrawCircle();
            if (Stream.Match(TokenType.DrawRectangle)) return ParseDrawRectangle();
            if (Stream.Match(TokenType.Fill)) return ParseFill();
            if (Stream.Match(TokenType.GoTo)) return ParseGoTo();
            return ParseExpressionStatement();
        }
        private Statement ParseSpawn()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Expression x = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression y = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new SpawnStmt(keyword, x, y);
        }

        private Statement ParseColor()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Expression color = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new ColorStmt(keyword, color);
        }
        private Statement ParseSize()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Expression size = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new SizeStmt(keyword, size);
        }
        private Statement ParseDrawLine()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Expression dirX = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression dirY = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression distance = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new DrawLineStmt(keyword, dirX, dirY, distance);
        }
        private Statement ParseDrawCircle()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Expression dirX = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression dirY = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression radius = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new DrawCircleStmt(keyword, dirX, dirY, radius);
        }
        private Statement ParseDrawRectangle()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Expression dirX = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression dirY = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression distances = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression width = ParseExpression();
            Consume(TokenType.Comma, "Expected ,");
            Expression height = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new DrawRectangleStmt(keyword, dirX, dirY, distances, width, height);
        }
        private Statement ParseFill()
        {
            Token keyword = Stream.Previous();
            Consume(TokenType.OpenBracket, "Expected (");
            Consume(TokenType.ClosedBracket, "Expected )");
            Consume(TokenType.EndOfLine, "Expected end of line");
            return new FillStmt(keyword);
        }

        private Statement ParseGoTo()
        {
            Consume(TokenType.OpenSquareBracket, "Expected [");
            Token label = Consume(TokenType.Label, "Expected label");
            Consume(TokenType.ClosedSquareBracket, "Expected ]");
            Consume(TokenType.OpenBracket, "Expected (");
            Expression expr = ParseExpression();
            Consume(TokenType.ClosedBracket, "Expected )");
            return new GoToStatement(label.Value, expr);
        }
        private Statement ParseLabel()
        {
            if (Stream.Current.Type == TokenType.Identifier && Stream.Peek().Type == TokenType.EndOfLine)
            {
                string label = Stream.Current.Value;
                Stream.Match(TokenType.Identifier);
                int line = Stream.Current.Line;
                Stream.Match(TokenType.EndOfLine);
                return new LabelStatement(label, line);
            }
            throw new SyntaxErrorException(Stream.Current.Line, "Invalid Label");
        }
        private Statement ParseDeclaration()
        {
            if (Stream.Peek().Type == TokenType.Identifier && Stream.Peek(2).Type == TokenType.Assign)
                return ParseVarDeclaration();
            return ParseExpressionStatement();
        }
        private Statement ParseVarDeclaration()
        {
            Token id = Stream.Next();
            Stream.Next();    //skip assignment operator
            Expression expr = ParseExpression();
            return new VarDeclaration(id.Value, expr);
        }
        private Statement ParseExpressionStatement()
        {
            Expression expr = ParseExpression();
            Stream.Match(TokenType.EndOfLine);
            return new ExpressionStatement(expr);
        }
        private Expression ParseExpression()
        {
            return ParseOr();
        }
        private Expression ParseOr()
        {
            Expression expr = ParseAnd();
            while (Stream.Match(TokenType.Or))
            {
                Token op = Stream.Previous();
                Expression right = ParseAnd();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }
        private Expression ParseAnd()
        {
            Expression expr = ParseEquality();
            while (Stream.Match(TokenType.And))
            {
                Token op = Stream.Previous();
                Expression right = ParseComparison();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }
        private Expression ParseEquality()
        {
            Expression expr = ParseComparison();
            while (Stream.Match(TokenType.Equal, TokenType.NotEqual))
            {
                Token op = Stream.Previous();
                Expression right = ParseOr();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }
        private Expression ParseComparison()
        {
            Expression expr = ParseAddition();
            while (Stream.Match(TokenType.BiggerOrEqual, TokenType.Bigger, TokenType.LesserOrEqual, TokenType.Lesser))
            {
                Token? op = Stream.Previous();
                Expression right = ParseAddition();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }
        private Expression ParseAddition()
        {
            Expression expr = ParseMultiplication();
            while (Stream.Match(TokenType.Addition, TokenType.Subtract))
            {
                Token op = Stream.Previous();
                Expression right = ParseMultiplication();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }
        private Expression ParseMultiplication()
        {
            Expression expr = ParseUnary();
            while (Stream.Match(TokenType.Multiplication, TokenType.Division, TokenType.Modulo, TokenType.Power))
            {
                Token op = Stream.Previous();
                Expression right = ParseUnary();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }
        private Expression ParseUnary()
        {
            if (Stream.Match(TokenType.Subtract, TokenType.Not))
            {
                Token op = Stream.Previous();
                Expression right = ParseUnary();
                return new UnaryExpression(op, right);
            }
            return ParseCall();
        }
        private Expression ParseCall()
        {
            Expression expr = ParsePrimary();
            if (Stream.Match(TokenType.OpenBracket))
                return FinishCall(expr);
            return expr;
        }
        private Expression ParsePrimary()
        {
            if (Stream.Match(TokenType.Number))
            {
                return new LiteralExpression(Stream.Previous());
            }
            if (Stream.Match(TokenType.Identifier))
            {
                return new VarExpression(Stream.Previous());
            }
            if (Stream.Match(TokenType.OpenBracket))
            {
                var expr = ParseExpression();
                if (!Stream.Match(TokenType.ClosedBracket))
                {
                    throw new SyntaxErrorException(Stream.Previous().Line, "Expected )");
                }
                return new GroupingExpression(expr);
            }
            throw new SyntaxErrorException(Stream.Current.Line, "Expected expression");
        }
        private void Synchronize()
        {
            while (!Stream.IsAtEnd())
            {
                if (Stream.Current.Type == TokenType.EndOfLine)
                {
                    Stream.Next();
                    break;
                }
                Stream.Next();
            }
        }
        private Expression FinishCall(Expression callee)
        {
            List<Expression> args = new();
            if (!Stream.Match(TokenType.ClosedBracket))
            {
                do
                {
                    args.Add(ParseExpression());
                } while (Stream.Match(TokenType.Comma));
            }
            Consume(TokenType.ClosedBracket, "Expected ) after arguments");
            if (callee is VarExpression varExpr)
                return new CallExpression(varExpr.Token, args);
            throw new SyntaxErrorException(Stream.Current.Line, "Invalid function call");
        }
    }
}