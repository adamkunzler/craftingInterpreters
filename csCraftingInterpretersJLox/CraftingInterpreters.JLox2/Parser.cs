using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingInterpreters.JLox2
{
    public class Parser
    {
        public class ParseError : Exception { };

        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr parse()
        {
            try
            {
                return expression();
            }
            catch(ParseError error)
            {
                return null;
            }
        }

        private Expr expression()
        {
            return equality();
        }

        private Expr equality()
        {
            var expr = comparison();
            while(match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var operatr = previous();
                var right = comparison();
                expr = new Expr.Binary(expr, operatr, right);
            }

            return expr;
        }

        private Expr comparison()
        {
            Expr expr = term();

            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var operatr = previous();
                var right = term();
                expr = new Expr.Binary(expr, operatr, right);
            }

            return expr;
        }

        private Expr term()
        {
            var expr = factor();

            while(match(TokenType.MINUS, TokenType.PLUS))
            {
                var operatr = previous();
                var right = factor();
                expr = new Expr.Binary(expr, operatr, right);
            }

            return expr;
        }

        private Expr factor()
        {
            var expr = unary();

            while(match(TokenType.SLASH, TokenType.STAR))
            {
                var operatr = previous();
                var right = unary();
                expr = new Expr.Binary(expr, operatr, right);
            }

            return expr;
        }

        private Expr unary()
        {
            if(match(TokenType.BANG, TokenType.MINUS))
            {
                var operatr = previous();
                var right = unary();
                return new Expr.Unary(operatr, right);
            }

            return primary();
        }

        private Expr primary()
        {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NIL)) return new Expr.Literal(null);

            if(match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(previous().literal);
            }

            if (match(TokenType.LEFT_PAREN))
            {
                var expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw error(peek(), "Expect expression");
        }

        private bool match(params TokenType[] types)
        {
            foreach(var type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }

        private bool check(TokenType type)
        {
            if(isAtEnd()) return false;
            return peek().type == type;
        }

        private Token advance()
        {
            if (!isAtEnd()) current++;
            return previous();
        }

        private bool isAtEnd()
        {
            return peek().type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }

        private ParseError error(Token token, string message)
        {
            Lox.error(token, message);
            return new ParseError();
        }

        private void synchronize()
        {
            advance();
            while (!isAtEnd())
            {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FOR:
                    case TokenType.FUN:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                    case TokenType.VAR:
                    case TokenType.WHILE:
                        return;
                }

                advance();
            }
        }
    }
}
