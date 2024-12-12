using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public List<Stmt> parse()
        {
            var statements = new List<Stmt>();
            while (!isAtEnd())
            {                
                try
                {
                    statements.Add(declaration());
                }
                catch (ParseError error)
                {
                    return null;
                }
            }

            return statements;
        }

        private Expr expression()
        {
            return assignment();
        }

        private Stmt declaration()
        {
            try
            {
                if (match(TokenType.FUN)) return function("function");
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch(ParseError error)
            {
                synchronize();
                return null;
            }
        }

        private Stmt statement()
        {
            if (match(TokenType.FOR)) return forStatement();
            if (match(TokenType.IF)) return ifStatement();
            if (match(TokenType.PRINT)) return printStatement();
            if (match(TokenType.RETURN)) return returnStatement();
            {
                
            }
            if (match(TokenType.WHILE)) return whileStatement();
            {
                
            }
            if (match(TokenType.LEFT_BRACE)) return new Stmt.Block(block());

            return expressionStatement();
        }

        private Stmt forStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt initializer;
            if (match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (match(TokenType.VAR))
            {
                initializer = varDeclaration();
            }
            else
            {
                initializer = expressionStatement();
            }

            Expr condition = null;
            if (!check(TokenType.SEMICOLON))
            {
                condition = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expr increment = null;
            if (!check(TokenType.RIGHT_PAREN))
            {
                increment = expression();
            }
            consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            var body = statement();

            if(increment != null)
            {
                body = new Stmt.Block(new List<Stmt>{ 
                    body,
                    new Stmt.Expression(increment)
                });
            }

            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if(initializer != null)
            {
                body = new Stmt.Block(new List<Stmt> { initializer, body });
            }

            return body;
        }

        private Stmt ifStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");

            var thenBranch = statement();
            Stmt elseBranch = null;
            if (match(TokenType.ELSE))
            {
                elseBranch = statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt printStatement()
        {
            var value = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt returnStatement()
        {
            var keyword = previous();
            Expr value = null;
            if (!check(TokenType.SEMICOLON))
            {
                value = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after return value.");

            return new Stmt.Return(keyword, value);
        }

        private Stmt whileStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            var body = statement();

            return new Stmt.While(condition, body);
        }

        private Stmt varDeclaration()
        {
            var name = consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (match(TokenType.EQUAL))
            {
                initializer = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt expressionStatement()
        {
            var expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt.Function function(string kind)
        {
            var name = consume(TokenType.IDENTIFIER, $"Expect {kind} name.");

            consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");

            var parameters = new List<Token>();
            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (match(TokenType.COMMA));
            }
            
            consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");

            var body = block();

            return new Stmt.Function(name, parameters, body);
        }

        private List<Stmt> block()
        {
            var statements = new List<Stmt>();

            while(!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Expr assignment()
        {
            var expr = or();

            if (match(TokenType.EQUAL))
            {
                var equals = previous();
                var value = assignment();

                if(expr.GetType() == typeof(Expr.Variable))
                {
                    var name = ((Expr.Variable)expr).name;
                    return new Expr.Assign(name, value);
                }

                error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr or()
        {
            var expr = and();

            while (match(TokenType.OR))
            {
                var operatr = previous();
                var right = and();
                expr = new Expr.Logical(expr, operatr, right);
            }

            return expr;
        }

        private Expr and()
        {
            var expr = equality();

            while (match(TokenType.AND)) 
            { 
                var operatr = previous();
                var right = equality();
                expr = new Expr.Logical(expr, operatr, right);
            }

            return expr;
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

            return call();
        }

        private Expr call()
        {
            var expr = primary();

            while (true)
            {
                if (match(TokenType.LEFT_PAREN))
                {
                    expr = finishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr finishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if(arguments.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 arguments.");
                    }

                    arguments.Add(expression());
                } while (match(TokenType.COMMA));
            }

            var paren = consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);
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

            if (match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(previous());
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
