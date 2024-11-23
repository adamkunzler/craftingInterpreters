using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingInterpreters.JLox2
{
    public class AstPrinter : Expr.Visitor<string>
    {
        public string print(Expr expr)
        {
            return expr.accept(this);
        }

        public string visitBinaryExpr(Expr.Binary expr)
        {
            return parenthesize(expr.operatr.lexeme, expr.left, expr.right);
        }

        public string visitGroupingExpr(Expr.Grouping expr)
        {
            return parenthesize("group", expr.expression);
        }

        public string visitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString();
        }

        public string visitUnaryExpr(Expr.Unary expr)
        {
            return parenthesize(expr.operatr.lexeme, expr.right);
        }

        private string parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach (var expr in exprs) 
            {
                sb.Append(" ");
                sb.Append(expr.accept(this));
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}
