using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingInterpreters.JLox2
{
    public class Interpreter : Expr.Visitor<object>
    {
        public void interpret(Expr expression)
        {
            try
            {
                var value = evaluate(expression);
                Console.WriteLine(stringify(value));
            }
            catch(RuntimeError error)
            {
                Lox.runtimeError(error);
            }
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object visitUnaryExpr(Expr.Unary expr) 
        {
            var right = evaluate(expr.right);

            switch (expr.operatr.type)
            {
                case TokenType.BANG:
                    return !isTruthy(right);
                case TokenType.MINUS:
                    checkNumberOperand(expr.operatr, right);
                    return -(double)right;
            }

            // unreachable
            return null;
        }

        public object visitBinaryExpr(Expr.Binary expr)
        {
            var left = evaluate(expr.left);
            var right = evaluate(expr.right);

            switch (expr.operatr.type)
            {
                case TokenType.GREATER:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL: return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL: return isEqual(left, right);
                case TokenType.MINUS:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if(left.GetType() == typeof(double) && right.GetType() == typeof(double))
                    {
                        return (double)left + (double)right;
                    }
                    if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeError(expr.operatr, "Operands must be two numbers or two strings");
                    
                case TokenType.SLASH:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expr.operatr, left, right);
                    return (double)left * (double)right;
                
            }

            // unreachable
            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.expression);
        }

        private object evaluate(Expr expr)
        {
            return expr.accept(this);
        }

        private bool isTruthy(object obj)
        {
            if (obj == null) return false;
            if(obj.GetType() == typeof(bool)) return (bool)obj;
            return true;
        }

        private bool isEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        private void checkNumberOperand(Token operatr, object operand)
        {
            if (operand.GetType() == typeof(double)) return;
            throw new RuntimeError(operatr, "Operand must be a number.");
        }

        private void checkNumberOperands(Token operatr, object left, object right)
        {
            if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) return;
            throw new RuntimeError(operatr, "Operands must be numbers.");
        }

        private string stringify(object obj)
        {
            if (obj == null) return "nil";

            if(obj.GetType() == typeof(double))
            {
                var text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }

    }
}
